import React, { useState } from "react";
import './SignUp.css';
import signUpImg from "../images/sign-in-bg.webp";
import  axios  from "axios";
import { useHistory } from "react-router-dom";
import Loader from "../shared/Loader";
import ErrorDialog from "../shared/ErrorDialog";
import { toast } from "react-toastify";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

export function SignUp () {
    const history = useHistory();
    const detailsBase = [
        {label : "UserType", required: true},
        {label : "Name", required: true},
        {label :"Email",required: true},
        {label: "Password", required: true}
    ]

    const base = {
        "UserType" : "",
        "Name" : "",
        "Email" : "",
        "Password" : ""
    }
    const [details, setUserDetails] = useState(base); 
    //const [errors, setErrors] = useState(["UserType","Name","Email","Password"]);
    const [errors, setErrors] = useState([]);
    const [errorMsg, setErrorMsg] = useState("");
    const [loading, setLoading] = useState(false);

    const setInputValue = (value, option) => {
        let currentDetails = {...details};
        currentDetails[option] = value;
        setUserDetails(currentDetails);
        console.log(currentDetails);
        //another way to do set individual properties in an object
        // setUserDetails({
        //     ...details,
        //     [option]: value
        // });
    }

    console.log(details);

    const handleUserSignUp = () => {
        details["isConfirmed"] = false;
        details["token"] = "";
        details["mfaSecret"] = "";
        console.log(details);
        setLoading(true);
        console.log(details);
        // const body = JSON.stringify(details); used when an object is passed for post api.
        //this can also used for post api
        //    axios.post('https://localhost:44356/post/user',body,
        //     {
        //         headers:
        //         {
        //             'Content-Type': 'application/json',
        //         }
        //     },
        //    )
        //axios.post('https://localhost:44356/post/user',details)
        axios.post('https://localhost:7253/post/user',details)
        .then((response) => {
            console.log(response);
            setUserDetails(
                {
                    "UserType" : "",
                    "Name" : "",
                    "Email" : "",
                    "Password" : ""
                }
            )
            setLoading(false);
            toast.success("User registered successfully, Please login Now!",{
                position: 'top-center',
                autoClose: 3000
            });
            // history.push("");
        })
        .catch((response) => {
            const apiData = response.response.data.error;
            setUserDetails(
                {
                    "UserType" : "",
                    "Name" : "",
                    "Email" : "",
                    "Password" : ""
                }
            )
            setLoading(false);
            setErrorMsg(apiData[0]);
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
    let check = filledFields.length === 4 && errors.length === 0 ? false: true;
    console.log(check);
    let invokeLoader = <Loader isLoading={loading}></Loader>
    const closeModal =()=>{
        setErrorMsg("");
      }
    console.log(errorMsg);
    let ErrorDialogMsg = errorMsg !== "" && (<ErrorDialog 
                            open={errorMsg !== ""}
                            ErrorTxt={"this Email Id already exists in database.Please use a different Email Id"}
                            ErrorTitle = {errorMsg}
                            handleDialogOk={closeModal}
                        >
                        </ErrorDialog>);

    const navigateToHome = () => {
        history.push("/Home");
    }
    return (
        <React.Fragment>
            {invokeLoader}
            {ErrorDialogMsg}
            <ToastContainer/>
            <div className={loading ? "signup-main faded" : "signup-main"}>
                <img src={signUpImg} alt="Room Mate Finder logo" className="signup-img"></img>
                <button className="close-btn" onClick={navigateToHome}>X</button>
                <div className="signup-parent">
                <div className="signup-section">
                    <div className="app-name">RoomMate <div style={{color:'green'}}>Finder</div></div>
                    <div className="app-name" style={{fontWeight:'normal', margin:'0'}}>Sign Up!</div>
                    <div style={{ fontFamily:'cursive',color:'GrayText', paddingTop:'5px' }}>Sign Up here by filling in the below details</div>
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
                    <button onClick={handleUserSignUp} disabled={check} className={`sign-up-btn ${check}`}>SignUp</button>
                </div>
                </div>
            </div>
        </React.Fragment>
    )
}