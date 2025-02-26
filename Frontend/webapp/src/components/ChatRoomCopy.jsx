import React from "react";
import { Component } from "react";
import signalRServiceInstance from "../shared/NotificationService2";
import { IconButton } from "@mui/material";
import SendIcon from '@mui/icons-material/Send';
import { rootReducer } from "../redux/store/store";
import { connect } from "react-redux";
import { setSelectedUserName } from "../redux/actions/SelectedUserName";
import { bindActionCreators } from "redux";
import store from "../redux/store/store";

class ChatRoomCopy extends Component {
    
    constructor(props) {
        super(props);
        this.state = {
            userMessage: "",
            roomLeft: false,
        }
        this.messagesEndRef = React.createRef();
        this.signalRMessage = store.getState();
    }

    componentDidUpdate = (prevProps) => {
        
        if (prevProps.signalRObject.SignalRMessage?.allMessages !== this.props.signalRObject.SignalRMessage?.allMessages) {
            this.scrollToBottom();
        }
    }

    scrollToBottom = () => {
        this.messagesEndRef.current?.scrollIntoView({ behavior: "auto" });
    };

    setUserMessage = (msg) => {
        this.setState({userMessage: msg});
    }

    sendMessage = () => {
        signalRServiceInstance.sendMessages(this.state.userMessage);
        this.setState({userMessage: ""});
    }

    handleLeaveRoom = () => {
        signalRServiceInstance.leaveChat(this.props.selectedUserName.SelectedUserName);
        this.setState({roomLeft: true});

    }

    render() {
        console.log(this.props.signalRObject.SignalRMessage);
        console.log(this.props.currentUserInfo.CurrentUserInfo?.name)
        return (
            <div className="gap">
                <div className="chat-room">
                        <div className="header-chat-room">
                            <div>Connected Users</div>
                            <div>Request Result: {this.props.signalRObject.SignalRMessage?.AcceptMessage ?? this.props.signalRObject.SignalRMessage?.RejectMessage}</div>
                            <div className="leave-chat" onClick={()=>this.handleLeaveRoom()}>Leave Chat</div>
                        </div>
                        
                        
                        <div className="users-actual-chat">
                            <div className="users-col">
                                {this.props.signalRObject.SignalRMessage?.connectedUsers?.map((x)=> {
                                    return (
                                        <div style={{color:'white'}}>{x}</div>
                                    )
                                })
                                }
                            </div>
                            <div className="chat-col">
                                <div className="auto-scroll">
                                    {this.props.signalRObject.SignalRMessage?.allMessages?.map((x) => {
                                        return (
                                            <div className="user-msg">
                                                <div className={x?.actualMessage ? "join-or-leave-msg": x?.userName === this.props.currentUserInfo.CurrentUserInfo?.name ? "current-user-msg" : "opposite-user-msg"}>{x.message}</div>
                                                <div className={x?.actualMessage ? "join-or-leave-msg date-text": x?.userName === this.props.currentUserInfo.CurrentUserInfo?.name ? "current-user-msg date-text" : "opposite-user-msg date-text"}>{x.formattedMessageTimeStamp}</div>
                                                <br></br>
                                            </div>    
                                        )
                                    })}
                                </div>
                                <div ref={this.messagesEndRef} />
                                
                                <div className="send-btn">
                                    <input type="text" placeholder="Type your message here..."value={this.state.userMessage} onChange={(e)=>this.setUserMessage(e.target.value)} disabled={this.state.roomLeft}></input>
                                    <IconButton onClick={()=>{this.sendMessage()}}>
                                    <SendIcon></SendIcon>
                                    </IconButton>
                                </div>
                            </div>
                        </div>
                    </div>
            </div>
        )
    }
}


const mapStateToProps = (state = rootReducer) => ({
    selectedUserName: state.SelectedUserName,
    signalRObject: state.SignalRMessage,
    currentUserInfo: state.CurrentUserInfo
})

const mapDispatchToProps = (dispatch) => ({
    setSelectedUserName: bindActionCreators(setSelectedUserName, dispatch),
})

export default connect(mapStateToProps,mapDispatchToProps)(ChatRoomCopy);