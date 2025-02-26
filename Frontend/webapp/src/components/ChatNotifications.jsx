import React from "react";
import store, { rootReducer } from "../redux/store/store";
import { connect } from "react-redux";
import signalRServiceInstance from "../shared/NotificationService2";
import { withRouter } from "react-router-dom";
import * as Utility from '../shared/Utility';
import './ChatNotifications.css';
import axios from "axios";


class ChatNotifications extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            acceptedRequest: false,
            matchedRequest: false,
            currentUserMatchedNames: [],
        }
        
    }

    acceptChatRequest = (fromUser) => {
        this.props.history.push(`/chat-with/user/${fromUser}`);
        signalRServiceInstance.acceptChatRequest(fromUser);
        const chatRoom = Utility.generateRoomName(fromUser,this.props.currentUserInfo.CurrentUserInfo?.name)
        signalRServiceInstance.joinRoom(this.props.currentUserInfo.CurrentUserInfo?.name, chatRoom)
    }

    rejectChatRequest = (fromUser) => {
        signalRServiceInstance.rejectChatRequest(fromUser);
    }

    acceptBookRequest = () => {
        signalRServiceInstance.acceptedBookRequest(this.props.signalRObject.SignalRMessage?.fromUser_Name, this.props.signalRObject.SignalRMessage?.toBeMatchedID);
        this.setState({acceptedRequest: true});
    }
    
    MatchBookRequest = () => {
        console.log("Heyy done")
        console.log(this.props.signalRObject.SignalRMessage?.ToUserAcceptedMessage);
        console.log(this.props.signalRObject.SignalRMessage.toBeMatchedID);
        console.log(this.props.signalRObject.SignalRMessage.toBeMatchedIDAccepted);
        console.log(store.getState())
        axios.get(`https://localhost:7253/match/user?ID=${this.props.signalRObject.SignalRMessage.ToBeMatchedIDAccepted}&fromUser=${this.props.currentUserInfo.CurrentUserInfo?.name}`
        //,{
            // headers:{
            //     'Authorization': `Bearer ${localStorage.getItem('userToken')}`
            // }
        //}
        )
        .then(()=> {
            this.setState({matchedRequest: true});
            console.log("yrue ues done")
        })
        .catch((err)=> console.log(err))
    }

    getMatchedUsers = () => {
        axios.get(`https://localhost:7253/get/matched/users?currentUserName=${this.props.currentUserInfo.CurrentUserInfo?.name}`
            //,{
                // headers:{
                //     'Authorization': `Bearer ${localStorage.getItem('userToken')}`
                // }
            //}
            )
            .then((res)=> {
                this.setState({ currentUserMatchedNames: [...res.data.data]});
            })
            .catch((err)=> console.log(err))
    }

    render() {
        console.log(this.props.signalRObject);
        return(
            <div className="gap">
                <div className="notification-panel">
                    <div className="main-chat-notification">
                        {
                            this.props.signalRObject.SignalRMessage?.msgsFromUsers?.map((x)=>{
                                return (
                                    <div className="sub-chat-notification">
                                        <div>{x.fromUserName}</div>
                                        <div onClick={()=>this.acceptChatRequest(x.fromUserName)}>{x["fromUser2Messgaes"][0]}</div>
                                        <div onClick={()=>this.rejectChatRequest(x.fromUserName)}>{x["fromUser2Messgaes"][1]}</div>
                                        {(this.props.signalRObject.SignalRMessage?.AcceptMessage) && 
                                        (<div>{this.props.signalRObject.SignalRMessage?.AcceptMessage}</div>)}
                                    </div>
                                )
                            })
                        }
                    </div>
                    <div>
                    {
                        this.props.signalRObject.SignalRMessage?.bookRequest && 
                        (
                            <>
                            <div>{this.props.signalRObject.SignalRMessage?.bookRequest}</div>
                            <button onClick={()=>{this.acceptBookRequest()}}>Accept</button>
                            {
                                this.state.acceptedRequest && 
                                (<div>Accepted to request from {this.props.signalRObject.SignalRMessage?.fromUser_Name} to match the room/room mate with 
                                record having ID {this.props.signalRObject.SignalRMessage?.toBeMatchedID} </div>)
                            }
                            </>
                        )
                    }
                    </div>
                    <div>
                    {
                        this.props.signalRObject.SignalRMessage?.ToUserAcceptedMessage && 
                        (
                            <>
                            <div>{this.props.signalRObject.SignalRMessage?.ToUserAcceptedMessage}</div>
                            <button onClick={()=>{this.MatchBookRequest()}}>Ok Complete</button>
                            {
                                this.state.acceptedRequest && 
                                (<div>Match Done</div>)
                            }
                            </>
                        )
                    }
                    </div>
                    <div>
                        <div>List of users with whom {this.props.currentUserInfo.CurrentUserInfo?.name} matched :</div>
                        <button onClick={()=>this.getMatchedUsers()}>get</button>
                        {this.state.currentUserMatchedNames?.length === 0 ?
                            (<></>)
                        :(<div>
                            {this.state.currentUserMatchedNames.map((name)=> {
                                return (<div>{name}
                                    </div>
                                )
                            })}
                        </div>)
                        }
                    </div>
                </div>
            </div>
        )
    }
}

const mapStateToProps = (state = rootReducer) => ({
    signalRObject: state.SignalRMessage,
    currentUserInfo: state.CurrentUserInfo
})

const mapDispatchToProps = (dispatch) => ({

})
export default connect(mapStateToProps,mapDispatchToProps)(withRouter(ChatNotifications));