import React from "react";
import { Component } from "react";
import './ChatRoom.css';
// import { signalRInstance } from "../shared/NotificationService2";
import { rootReducer } from "../redux/store/store";
import { connect } from "react-redux";
import store from "../redux/store/store";
import { colors, IconButton, THEME_ID } from "@mui/material";
import SendIcon from '@mui/icons-material/Send';
import signalRServiceInstance from "../shared/NotificationService2";



class ChatRoom extends Component {
    constructor(props) {
        super(props);
        this.state = {
            userName: "",
            ChatRoom: "",
            joinRoom: false,
            userMessage: "",
            roomLeft: false,
        }
    }

    componentDidMount() {

    }

    componentDidUpdate() {

    }

    setUserName = (value) => {
        this.setState({userName:value})
    }

    setChatRoom = (value)=>{
        this.setState({ChatRoom: value})
    }

    setUserMessage = (msg) => {
        this.setState({userMessage: msg});
    }

    sendMessage = () => {
        signalRServiceInstance.sendMessages(this.state.userMessage);
        this.setState({userMessage: ""});
    }


    joinRoom = () => {
        this.setState({joinRoom : true})
        signalRServiceInstance.joinRoom(this.state.userName,this.state.ChatRoom);
    }

    handleLeaveRoom = () => {
        signalRServiceInstance.leaveChat(this.state.ChatRoom);
        this.setState({roomLeft: true});
    }

    render() {
        console.log(store.getState());
        console.log(this.props.signalRObject.SignalRMessage);
        return (
            <div className="gap">
                {!this.state.joinRoom ? 
                    (<div className="form">
                        <label>UserName:</label>
                        <input type="text" onChange={(e) => {this.setUserName(e.target.value)}} value={this.state.userName}/>
                        <label>ChatRoom:</label>
                        <input type="text" onChange={(e) => {this.setChatRoom(e.target.value)}} value={this.state.ChatRoom}/>
                        <button onClick={()=>this.joinRoom()} className="join-room-btn">Join Room</button>
                    </div>)
                    :
                    (<div className="chat-room">
                        <div className="header-chat-room">
                            <div>Connected Users</div>
                            <div className="leave-chat" onClick={()=>this.handleLeaveRoom()}>Leave Chat</div>
                        </div>
                        
                        
                        <div className="users-actual-chat">
                            <div className="users-col">
                                {this.props.signalRObject.SignalRMessage.connectedUsers?.map((x)=> {
                                    return (
                                        <div style={{color:'white'}}>{x}</div>
                                    )
                                })
                                }
                            </div>
                            <div className="chat-col">
                                <div>
                                    {this.props.signalRObject.SignalRMessage.allMessages?.map((x) => {
                                        return (
                                            <div className="user-msg">
                                                <div className={x.actualMessage ? "join-or-leave-msg": x.userName === this.state.userName ? "current-user-msg" : "opposite-user-msg"}>{x.recievedMessage}</div>
                                                <div className={x.actualMessage ? "join-or-leave-msg date-text": x.userName === this.state.userName ? "current-user-msg date-text" : "opposite-user-msg date-text"}>{x.recievedTimeSTamp}</div>
                                                <br></br>
                                            </div>
                                            
                                        )
                                    })}
                                </div>
                                <div className="send-btn">
                                    <input type="text" placeholder="Type your message here..."value={this.state.userMessage} onChange={(e)=>this.setUserMessage(e.target.value)} disabled={this.state.roomLeft}></input>
                                    <IconButton onClick={()=>{this.sendMessage()}}>
                                    <SendIcon></SendIcon>
                                    </IconButton>
                                </div>
                            </div>
                        </div>
                    </div>)
                }

            </div>
        )
    }
}

const mapStateToProps = (state=rootReducer) => ({
    signalRObject: state.SignalRMessage
})

const mapDispatchToProps = (dispatch) => ({
})

export default connect(mapStateToProps,mapDispatchToProps)(ChatRoom);