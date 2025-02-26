import * as signalR from '@microsoft/signalr';
import store from '../redux/store/store';
import { setRefreshScreenData } from '../redux/actions/RefreshScreenData';

export const InitializeSignalR = async () => {
    const connection =  
    new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7253/notificationHub", {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        })
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect().build();

    connection.start()
        .then(()=>{
            console.log("Connection established");
            store.dispatch(setRefreshScreenData({shouldRefreshAllRoomsPage: false}))
        })
        .catch((err)=>{
            console.log(err);
        })
    
    //for tracking changes in RoomDetails Container in Cosmos DB
    connection.on('ChangedRoomDetails', (data) => {
        if(data && data?.length > 0){
            store.dispatch(setRefreshScreenData({shouldRefreshAllRoomsPage: true}))
        }
    });

    //for tracking changes in RoomDescription Container in Cosmos DB
    connection.on('ChangedRoomDescription', (data) => {
        if(data && data?.length > 0){
            console.log("hey done");
            //store.dispatch(setRefreshScreenData({shouldRefreshAllRoomsPages: true}))
        }
    });
    connection.onreconnecting(error => {
        console.log(`SignalR reconnecting. Reason: ${error}`);
    });

    // Listen for successful reconnection
    connection.onreconnected(connectionId => {
        console.log(`SignalR reconnected successfully. Connection ID: ${connectionId}`);
    });

    // Handle the connection closing
    connection.onclose(error => {
        console.log(`SignalR connection closed. Error: ${error}`);
    });

    //join the chat room
    const joinRoom = (userName, chatRoom) => {
        return connection.invoke("JoinRoom",{userName,chatRoom})
    }
    

    //send the messages
    const SendMessages = (Message) => {
        connection.invoke("SendMessage",Message);
    }

    //leave the chat Room
    const leaveChat = () => {
        connection.stop();
    }

    connection.on("RecieveMessage",(userName, message, MessageTime)=> {
        console.log(userName)
        console.log(message)
        console.log(MessageTime)
    })

    connection.on("ConnectedUser", (users) => {
        console.log(users)
    })
}