import { bindActionCreators } from "redux";
import { setCheckCode } from "../redux/actions/CheckAction";
import store2, { AllReducers } from "../redux/store/store2";
import { connect } from "react-redux";
import React from "react";
import InterviewPrep3 from "./InterviewPrep3";
import { withRouter } from "react-router-dom/cjs/react-router-dom.min";

class InterviewPrep2 extends React.Component {

    /**
     *
     */
    constructor(props) {
        super(props);
        this.state = {
            patientsData : []
        }
    }

    handleSend = () => {
        this.props.setCheckCode(104);

        const apiCall = new Promise((resolve, reject)=>{
            let abc = 90;
            console.log(store2.getState())
            if(store2.getState().CheckAction.CheckAction) {
                resolve("Its done");
            }
            else {
                reject("not done");
            }
        })
        apiCall
        .then((response) => {console.log(response)})
        .catch((error) => console.log(error));
    }

    handleCreateAnAPICall = () => {
        const apiResult = new Promise((resolve,reject)=>{
            if(true) {
                resolve("api is resolved");
            }
            else {
                reject("api is not resolved");
            }
        })
        apiResult.then((result) => {console.log(result)
            this.setState({patientsData: result});
        })
        .catch((error)=>console.log(error));
    }

    handleUnsend = () => {
        this.props.setCheckCode(-90);
    }

    handleParent = (data) => {
        console.log(this.props.history);
        console.log(this.props.location);
        console.log(data,"Data sent from child");
        this.props.history.push({
            pathname: "/i/me/myslef",
            search: "2345678"
        })
        window.open('/','_self');
    }

    render() {
        console.log(this.props);
        console.log(this.props.history);
        console.log(this.props.location);

        return(
            <div>
            <div style={{ position:'absolute', top:'100px'}}>
                {this.props.checkAction?.CheckAction};
                <button onClick={()=>{this.handleSend()}}>Send</button>
                <button onClick={()=>{this.handleUnsend()}}>unsend</button>
                <InterviewPrep3 handleClick={this.handleParent}></InterviewPrep3>
            </div>
            <div>
                {this.state.patientsData && 
                (<div>
                    {this.state.patientsData.map((x)=>{
                        return (<div>{x}</div>)
                    })}
                    </div>)} 
            </div>
            </div>
        )
    }

}

const mapStateToProps = (state=AllReducers) => ({
    checkAction: state.CheckAction,
})

const mapDispatchToProps = (dispatch) => ({
    setCheckCode : bindActionCreators(setCheckCode, dispatch),
})

export default connect(mapStateToProps,mapDispatchToProps)(withRouter(InterviewPrep2));