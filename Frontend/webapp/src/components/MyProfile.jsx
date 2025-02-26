import React from "react";
import store, { rootReducer } from "../redux/store/store";
import './MyProfile.css';
import axios from "axios";
import { toast } from "react-toastify";
import { connect } from "react-redux";
import { bindActionCreators } from "redux";
import { setCurrentUserInfo } from "../redux/actions/CurrentUserInfo";
import Button from "@mui/material/Button";
import { InputLabel } from "@mui/material";
import DCCBreadcrumbs from "../shared/DCCBreadcrumbs";


class MyProfile extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            changedData: store.getState().CurrentUserInfo.CurrentUserInfo,
            saveChangesDisabled: true,
        }
        this.data = store.getState();
    }

    // componentDidMount() {
    //     if(Object.keys(this.props.currentUserInfo.CurrentUserInfo).length === 0) {
    //         throw new Error('I crashed');
    //     }
    // }

    handleChangeUserData = (elementValue,elementKey) => {
        this.setState({changedData: {...this.state.changedData, [elementKey]: elementValue}});
    }

    handleButtonDisability = () => {
        let changedDataKeys = Object.keys(this.state.changedData);
        let check = changedDataKeys.every((x)=>this.state.changedData[x] === store.getState().CurrentUserInfo.CurrentUserInfo[x]);
        this.setState({saveChangesDisabled: check});
        console.log(check);
        return check;
    }

    handleSaveChanges = () => {
        console.log(this.state.changedData);
        //axios.put('https://localhost:44356/put/user',this.state.changedData,{
        axios.put('https://localhost:7253/put/user',this.state.changedData,{
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('userToken')}`
            }
        })
        .then((response) => {
            console.log(response);
            console.log(response.data);
            let modifiedUserProfile = {...this.state.changedData};
            this.props.setCurrentUserInfo(modifiedUserProfile);
            toast.success("Profile updated successfully !",{
                position: 'top-center',
                autoClose: 3000
            });

        })
        .catch((error) => {
            console.log(error);
        })
    }

    handleDeleteUser = () => {
        //axios.delete(`https://localhost:44356/delete/user/?email=${this.state.changedData.email}`,
        axios.delete(`https://localhost:7253/delete/user/?email=${this.state.changedData.email}`,
            {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('userToken')}`
                }
        })
        .then((response)=>{
                this.props.setCurrentUserInfo({});
                this.setState({changedData: {
                    "email": "",
                    "name": "",
                    "usertype":"",
                    "password": ""
                }});
                localStorage.setItem('userToken',"");
                toast.success("Profile Deleted Successfully !",{
                    position: 'top-center',
                    autoClose: 3000
                });
            }
        )
        .catch((error)=>{console.log(error)})
    }
    
    render() {
        // if(Object.keys(this.props.currentUserInfo.CurrentUserInfo).length === 0) {
        //     throw new Error('I crashed');
        // }
        console.log(this.props);
        console.log(this.props.currentUserInfo.CurrentUserInfo?.name)
        console.log(localStorage.getItem('userToken'));
        let userInfo = store.getState().CurrentUserInfo.CurrentUserInfo;
        console.log(this.state.changedData);
        let changedDataKeys = Object.keys(this.state.changedData);
        let check = changedDataKeys.every((x)=>this.state.changedData[x] === store.getState().CurrentUserInfo.CurrentUserInfo[x]);
        console.log(check);
        let allInputs = ["name","email","usertype","password"];
        return(
            <div className="up-gap">
                <div className="my-profile-parent">
                <DCCBreadcrumbs
                        parts = {['Home','/places','My Profile',this.props.currentUserInfo.CurrentUserInfo?.name]}
                />
                    <div className="my-profile">
                        <div style={{color:'grey',fontWeight:'700',fontSize:'25px',fontStyle:'italic',textAlign:'center'}}>Your Profile</div>
                        <div className="hrLine"></div>
                        <div className="only-input-btns">
                            <div className="all-inputs">
                                {allInputs.map((x) => {
                                    return (
                                        <div className="input-box">
                                            <InputLabel>{x.toLocaleUpperCase()}</InputLabel>
                                            <input type="text" value={this.state.changedData[x]} onChange={(event)=>this.handleChangeUserData(event.target.value,x)}/>
                                        </div>)
                                })}
                                
                                {/* <input type="text" value={userInfo.email}/>
                                <input type="text" value={userInfo.usertype}/>
                                <input type="text" value={userInfo.password}/> */}
                            </div>
                        </div>
                        <div style={{display:'flex',justifyContent:'center',alignItems:'center'}}><button disbaled={check} onClick={this.handleSaveChanges} className={`save-changes-btn ${check} `}>Save Changes</button></div>
                        <div className="hrLine"></div>
                        <div style={{paddingLeft:'39px',color:'grey',fontSize:'small'}}>
                            if you are no longer interested in using your account click the button below to delete your account.
                        </div>
                        <div style={{display:'flex',justifyContent:'center',alignItems:'center'}}>
                        <button onClick={this.handleDeleteUser} className={`delete-btn`}>Delete</button>
                        </div>
                    </div>
                </div>
                {/* <button disbaled={this.handleButtonDisability} onClick={this.handleSaveChanges} className={`save-changes-btn ${this.state.saveChangesDisabled}`}>Save Changes</button> */}
                
            </div>
        )
    }
}
const mapStateToProps = (state=rootReducer) => ({
    currentUserInfo: state.CurrentUserInfo
})
const mapDispatchToProps = (dispatch) => ({
    setCurrentUserInfo: bindActionCreators(setCurrentUserInfo,dispatch)
})

export default connect(mapStateToProps,mapDispatchToProps)(MyProfile);