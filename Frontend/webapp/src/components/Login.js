import React, { useState,useContext } from "react";
import './SignUp.css';
import signUpImg from "../images/sign-in-bg.webp";
import  axios  from "axios";
import { useHistory } from "react-router-dom";
import Loader from "../shared/Loader";
import ErrorDialog from "../shared/ErrorDialog";
import { UserContext } from "./UserContext";

export function Login () {
    const history = useHistory();
    const detailsBase = [
        {label :"Email",required: true},
        {label: "Password", required: true}
    ]

    const base = {
        "Email" : "",
        "Password" : ""
    }
    const [details, setUserDetails] = useState(base); 
    const [errors, setErrors] = useState([]);
    const [errorMsg, setErrorMsg] = useState("");
    const [loading, setLoading] = useState(false);
    const [[userData,setUserData],[firstLogin,setFirstLogin]] = useContext(UserContext);



    const setInputValue = (value, option) => {
        let currentDetails = {...details};
        currentDetails[option] = value;
        setUserDetails(currentDetails);
        console.log(currentDetails);
    }

    console.log(details);    

    const handleUserSignUp = () => {
        setLoading(true);
        axios.post('https://localhost:44356/validate/user',details)
        .then((response) => {
            if(response.data.token) {
                localStorage.setItem('userToken',response.data.token);
            }
            console.log(response);
            setLoading(false);
            setUserData(
                {...userData,
                    UserType: response?.data?.data?.usertype,
                    Name: response?.data?.data?.name,
                    Email: response?.data?.data?.email
                }
            )
            setFirstLogin(true);
            setUserDetails(
                {
                    "Email" : "",
                    "Password" : ""
                }
            )
            history.push("/places");
        })
        .catch((response) => {
            const apiData = response.response.data.error;
            setUserDetails(
                {
                    "Email" : "",
                    "Password" : ""
                }
            )
            setLoading(false);
            setErrorMsg(apiData);
            console.log(details);
        })
    }

    const hanldeOnBlur = (value,label) => {
        let errorList = [...errors];
        if(value === "") {
            errorList.push(label);
            setErrors(errorList);
        }
        if(value !== "" && errorList.includes(label)) {
            errorList = errorList.filter((item)=> {
                return item !== label
            })
            setErrors(errorList);
        }
    }
    console.log(errors);
    //let check = errors.length === 0 ? false : true;
    
    let filledFields = Object.values(details).filter((element)=>{
        return (element !== "")
    })
    console.log(filledFields);
    let check = filledFields.length === 2 && errors.length === 0 ? false: true;
    console.log(check);
    let invokeLoader = <Loader isLoading={loading}></Loader>
    const closeModal =()=>{
        setErrorMsg("");
    }
    console.log(errorMsg);
    let ErrorDialogMsg = errorMsg !== "" && (<ErrorDialog 
                            open={errorMsg !== ""}
                            ErrorTxt={errorMsg}
                            ErrorTitle = {""}
                            handleDialogOk={closeModal}
                        >
                        </ErrorDialog>);

    const navigateToHome = () => {
        history.push("/");
    }
    return (
        <React.Fragment>
            {invokeLoader}
            {ErrorDialogMsg}
            <div className={loading ? "signup-main faded" : "signup-main"}>
                <img src={signUpImg} alt="Room Mate Finder logo" className="signup-img"></img>
                <button className="close-btn" onClick={navigateToHome}>X</button>
                <div className="signup-parent">
                <div className="signup-section">
                    <div className="app-name">RoomMate <div style={{color:'green'}}>Finder</div></div>
                    <div className="app-name" style={{fontWeight:'normal', margin:'0'}}>Login!</div>
                    <div style={{ fontFamily:'cursive',color:'GrayText', paddingTop:'5px' }}>Login here by filling in the below details</div>
                    <div className="options-main">
                        {detailsBase.map((element)=>{ 
                            return (
                                <div className="option">
                                    <div style={{width: '100px'}}>{element.label} : </div>
                                    <input
                                        onBlur = {(e)=>{hanldeOnBlur(e.target.value,element.label)}}
                                        type="text"
                                        className="input"
                                        value={details[element.label]}
                                        placeholder={`Enter ${element.label}`} 
                                        onChange={(e)=>setInputValue(e.target.value,element.label)}
                                    >
                                    </input>
                                    {(errors.includes(element.label) && element.required === true)&& 
                                    <span className="onBlur-msg">{element.label} is required</span>}
                                </div>
                            )
                        })}
                    </div>
                    <button onClick={handleUserSignUp} disabled={check} className={`sign-up-btn ${check}`}>Login</button>
                </div>
                </div>
            </div>
        </React.Fragment>
    )
}