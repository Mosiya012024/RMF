import React, { Component } from 'react';
import * as signalR from '@microsoft/signalr';
import store from '../redux/store/store';
import { setRefreshScreenData } from '../redux/actions/RefreshScreenData';
import { setSignalRMessage } from '../redux/actions/SignalRMessage';


class NotificationService2 extends Component {
    constructor(props) {
        super(props);
        this.connection = null; // SignalR connection
        this.data = store.getState();
    }
    

    initializeSignalR = async () => {
        const token = localStorage.getItem('userToken');
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7253/notificationHub", {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets,
                accessTokenFactory: () => token
            })
            .configureLogging(signalR.LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        this.connection.start()
            .then(() => {
                console.log("Connection established");
                store.dispatch(setRefreshScreenData({ shouldRefreshAllRoomsPage: false }));
            })
            .catch((err) => {
                console.log(err);
            });

        this.connection.on('ChangedRoomDetails', (data) => {
            if (data && data?.length > 0) {
                store.dispatch(setRefreshScreenData({ shouldRefreshAllRoomsPage: true }));
            }
        });

        this.connection.on('ChangedRoomDescription', (data) => {
            if (data && data?.length > 0) {
                console.log("Room description changed");
            }
        });

        this.connection.onreconnecting(error => {
            console.log(`SignalR reconnecting. Reason: ${error}`);
        });

        this.connection.onreconnected(connectionId => {
            console.log(`SignalR reconnected successfully. Connection ID: ${connectionId}`);
        });

        this.connection.onclose(error => {
            console.log(`SignalR connection closed. Error: ${error}`);
        });

        this.connection.on("RecieveMessageDuplicate", (userName, message, MessageTime) => {
            console.log(userName);
            console.log(message);
            console.log(MessageTime);
            this.setAllMessages(message,userName, MessageTime,false);
            // const currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage || {};
            // // const updatedSignalRMessage = {
            // //     ...currentSignalRMessage, userName: userName, message: message, MessageTime: MessageTime
            // // }
            // let currentAllMessages = store.getState().SignalRMessage.SignalRMessage.allMessages;
            // console.log(currentAllMessages);
            // let currentMessages = currentAllMessages === undefined ? [] : [...currentAllMessages];
            // currentMessages.push({recievedMessage : message, recievedTimeSTamp: MessageTime, actualMessage: false})
            // let updatedSignalRMessages = {
            //     ...currentSignalRMessage, allMessages: currentMessages 
            // }
            // console.log(updatedSignalRMessages);
            // store.dispatch(setSignalRMessage(updatedSignalRMessages));
            //this.props.setSignalRMessage({...this.props.signalRMessages.SignalRMessage, userName: userName, message: message, MessageTime:MessageTime})

            //store.dispatch(setSignalRMessage({...this.props.signalRMessages.SignalRMessage, userName: userName, message: message, MessageTime:MessageTime}));
        });

        this.connection.on("RecieveMessage", (formattedRecievedMessages) => {
            this.setFormattedRecievedMessages(formattedRecievedMessages);
        });

        this.connection.on("ConnectedUser", (users) => {
            const currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage || {};
            const updatedSignalRMessage = {
                ...currentSignalRMessage, connectedUsers: users
            }
            console.log(users);
            store.dispatch(setSignalRMessage(updatedSignalRMessage));
            // this.props.setSignalRMessage({...this.props.signalRMessages.SignalRMessage, connectedUsers: [...users]});
            //store.dispatch(setSignalRMessage({...currentUser, connectedUsers: [...users]}));

        });

        this.connection.on("JoinRoomMessage", (joinRoomMessage,MessageTime) => {
            console.log(joinRoomMessage);
            console.log(MessageTime);
            this.setAllMessages(joinRoomMessage,"", MessageTime,true);
            // const currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage || {};
            // const updatedSignalRMessage = {
            //     ...currentSignalRMessage, joinRoomMessage: joinRoomMessage
            // }
            // store.dispatch(setSignalRMessage(updatedSignalRMessage));
            //this.props.setSignalRMessage({...this.props.signalRMessages.SignalRMessage, connectedUsers: [...users]});
            //store.dispatch(setSignalRMessage({...this.props.signalRMessages.SignalRMessage, connectedUsers: [...users]}));

        });

        this.connection.on("LeaveRoomMessage", (leaveRoomMessage, MessageTime) => {
            console.log(leaveRoomMessage);
            this.setAllMessages(leaveRoomMessage,"", MessageTime,true);
            //this.props.setSignalRMessage({...this.props.signalRMessages.SignalRMessage, connectedUsers: [...users]});
            //store.dispatch(setSignalRMessage({...this.props.signalRMessages.SignalRMessage, connectedUsers: [...users]}));

        });

        this.connection.on("SendChatRequestMethodResonse", (fromUser,data1, data2) => {
            console.log(fromUser);
            console.log(data1);
            console.log(data2);
            console.log(fromUser);
            
            let currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage;
            let currentMsgsFromUsers = currentSignalRMessage?.msgsFromUsers;
            let mutlipleFromUsers = currentMsgsFromUsers == undefined ? [] : currentMsgsFromUsers;
            let requestMsgs = [];
            requestMsgs.push(data1);
            requestMsgs.push(data2);
            mutlipleFromUsers.push({
                fromUserName: fromUser,
                fromUser2Messgaes: requestMsgs
            });
            console.log(mutlipleFromUsers);
            
            let updatedSignalRMessage = {
                ...currentSignalRMessage, msgsFromUsers: mutlipleFromUsers
            };
            console.log(updatedSignalRMessage);
            store.dispatch(setSignalRMessage(updatedSignalRMessage));
        })

        this.connection.on("AcceptChatRequestMethodResponse", (data) => {
            let currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage;
            let updatedSignalRMessage = {
                ...currentSignalRMessage, AcceptMessage: data
            };
            console.log(updatedSignalRMessage);
            store.dispatch(setSignalRMessage(updatedSignalRMessage));
        })

        this.connection.on("RejectChatRequestMethodResponse", (data) => {
            let currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage;
            let updatedSignalRMessage = {
                ...currentSignalRMessage, RejectMessage: data
            };
            console.log(updatedSignalRMessage);
            store.dispatch(setSignalRMessage(updatedSignalRMessage));
        })

        this.connection.on("IsFirstJoined", (check) => {
            console.log(check);
            let currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage;
            let updatedSignalRMessage = {
                ...currentSignalRMessage, isFirstJoined: check
            };
            console.log(updatedSignalRMessage);
            console.log('After dispatch:', store.getState().SignalRMessage);
            store.dispatch(setSignalRMessage(updatedSignalRMessage));
            console.log('After dispatch:', store.getState().SignalRMessage);
        })

        this.connection.on("AcceptOrRejectBookRequest", (data1,data2,data3) => {
            console.log(data1, data2);
            let currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage;
            let updatedSignalRMessage = {
                ...currentSignalRMessage, bookRequest: data1, fromUser_Name: data2, toBeMatchedID: data3
            };
            console.log(updatedSignalRMessage);
            console.log('After dispatch: abcdefg', store.getState().SignalRMessage);
            store.dispatch(setSignalRMessage(updatedSignalRMessage));
            console.log('After dispatch: abcdefg', store.getState().SignalRMessage);
        })
        this.connection.on("ToUserAccepted",(data1,data2)=>{
            console.log(data1, data2);
            let currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage;
            let updatedSignalRMessage = {
                ...currentSignalRMessage, ToUserAcceptedMessage: data1, ToBeMatchedIDAccepted: data2
            };
            console.log(updatedSignalRMessage);
            console.log('After dispatch: abcdefg', store.getState().SignalRMessage);
            store.dispatch(setSignalRMessage(updatedSignalRMessage));
            console.log('After dispatch: abcdefg', store.getState().SignalRMessage);
        })

        
    };

    joinRoom = (userName, chatRoom) => {
        let currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage;
        let updatedSignalRMessage = {
            ...currentSignalRMessage,AcceptMessage: null
        };

        store.dispatch(setSignalRMessage({updatedSignalRMessage}));
        return this.connection.invoke("JoinRoom", { userName, chatRoom });
    }

    sendMessages = (Message) => {
        this.connection.invoke("SendMessage", Message);
    }

    leaveChat = (chatRoom) => {
        this.connection.invoke("LeaveRoom",chatRoom);
    }

    acceptChatRequest = (fromUser) => {
        this.connection.invoke("AcceptChatRequest",fromUser);
    }

    rejectChatRequest = (fromUser) => {
        this.connection.invoke("RejectChatRequest",fromUser);
    }

    sendChatRequest = async (toUser,fromUser) => {
        await this.connection.invoke("SendChatRequest", toUser, fromUser);
    }

    ChatRequest = (fromUser) => {
        this.connection.invoke("AcceptChatRequest",fromUser);
    }

    checkFirstJoined = async (currentUser,chatRoom) => {
        await this.connection.invoke("CheckJoinedFirstTime", currentUser,chatRoom);
        return store.getState().SignalRMessage.SignalRMessage?.isFirstJoined;
    }

    sendBookRequest = (toUser, fromUser, toBeMatchedID) => {
        this.connection.invoke("SendBookRequest",toUser, fromUser, toBeMatchedID);
    }

    acceptedBookRequest = (fromUser_Name, toBeMatchedID) => {
        this.connection.invoke("AcceptedBookRequest",fromUser_Name, toBeMatchedID);
    }
    setFormattedRecievedMessages = (formattedRecievedMessages) => {
        console.log(formattedRecievedMessages);
        const currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage || {};
        
        let currentAllMessages = store.getState().SignalRMessage.SignalRMessage?.allMessages;
        //console.log(currentAllMessages[0]);
        let currentMessages = [];
        if(currentAllMessages !== undefined) {
            currentMessages.push(currentAllMessages[0]);
        }
        formattedRecievedMessages.forEach(element => {
            currentMessages.push(element);
        });
        
        console.log(currentMessages);
        let updatedSignalRMessages = {
            ...currentSignalRMessage, allMessages: currentMessages
        }
        console.log(updatedSignalRMessages);
        store.dispatch(setSignalRMessage(updatedSignalRMessages));

    }

    setAllMessages = (message,userName,MessageTime, isActualMessage) => {
        console.log(message)
        console.log(MessageTime)
        console.log(isActualMessage);
        const currentSignalRMessage = store.getState().SignalRMessage.SignalRMessage || {};
        
        let currentAllMessages = store.getState().SignalRMessage.SignalRMessage?.allMessages;
        console.log(currentAllMessages);
        let currentMessages = currentAllMessages === undefined ? [] : [...currentAllMessages];
        currentMessages.push({message : message, userName: userName, formattedMessageTimeStamp: MessageTime, actualMessage: isActualMessage})
        let updatedSignalRMessages = {
            ...currentSignalRMessage, allMessages: currentMessages
        }
        console.log(updatedSignalRMessages);
        store.dispatch(setSignalRMessage(updatedSignalRMessages));
    }

}

const signalRServiceInstance = new NotificationService2();
export default signalRServiceInstance;
//export default signalRInstance;
// const mapStateToProps = (state=rootReducer) => ({
//     signalRMessages : state.SignalRMessage
// })

// const mapDispatchToProps = (dispatch) => ({
//     setSignalRMessage : bindActionCreators(setSignalRMessage,dispatch)
// })


// export default connect(mapStateToProps, mapDispatchToProps)(NotificationService2);
//const ConnectedNotificationService = connect(mapStateToProps, mapDispatchToProps)(NotificationService2);

// class NotificationServiceSingleton {
//     constructor() {
//         if (!NotificationServiceSingleton.instance) {
//             this.notificationService = new ConnectedNotificationService();
//             NotificationServiceSingleton.instance = this;
//         }
//         return NotificationServiceSingleton.instance;
//     }

//     getNotificationServiceInstance() {
//         return this.notificationService;
//     }
// }

// // Export singleton instance
// const notificationServiceInstance = new NotificationServiceSingleton();
// export default notificationServiceInstance;

