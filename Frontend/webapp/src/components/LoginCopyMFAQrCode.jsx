import React, { useState,useContext } from "react";
import './SignUp.css';
import signUpImg from "../images/sign-in-bg.webp";
import  axios  from "axios";
import { useHistory } from "react-router-dom";
import Loader from "../shared/Loader";
import ErrorDialog from "../shared/ErrorDialog";
import { UserContext } from "./UserContext";
import { rootReducer } from "../redux/store/store";
import { connect, useDispatch } from "react-redux";
import { setCurrentUserInfo } from "../redux/actions/CurrentUserInfo";
import { bindActionCreators } from "redux";
import store from "../redux/store/store";
import { setAPIResponseCode } from "../redux/actions/APIResponseCode";

function LoginMFAQrCode () {
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
    const dispatch = useDispatch();
    const [isOtpRequired, setIsOtpRequired] = useState(false);
    const [otpValue, setOtpValue] = useState("");
    const [keyString, setKeyString] = useState("");


    const setInputValue = (value, option) => {
        let currentDetails = {...details};
        currentDetails[option] = value;
        setUserDetails(currentDetails);
        console.log(currentDetails);
    }

    console.log(details);    

    const OnChangeOTPValue = (otp) =>{
        setOtpValue(otp);
    }
    const ValidateUserAndSendOTP = () => {
        axios.post('https://localhost:7253/validateuserAndGenerateQR/user',details)
        .then((result)=>{
            if(result.data.code === 200) {
                setKeyString(result.data.data);
                setIsOtpRequired(true);
            }
        })
    }

    const VerifyOTP = () => {
        const otpRequest = {
            "UserId": details.Email,
            "OtpCode": otpValue
        }
        axios.post('https://localhost:7253/validate/user',otpRequest)
        .then((response) => {
            if(response.data.token) {
                localStorage.setItem('userToken',response.data.token);
            }
            console.log(response);
            setLoading(false);
            // setUserData(
            //     {...userData,
            //         UserType: response?.data?.data?.usertype,
            //         Name: response?.data?.data?.name,
            //         Email: response?.data?.data?.email
            //     }
            // )


            console.log(response);
            console.log(response?.data);
            console.log(response?.data?.data);
            let loggedInUserData = {...response?.data?.data};
            // this.props.setCurrentUserInfo({...loggedInUserData})
            console.log(loggedInUserData);
            dispatch(setCurrentUserInfo(loggedInUserData));
            dispatch(setAPIResponseCode(0));
            console.log(store.getState())
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
    const handleUserSignUp = () => {
        setLoading(true);
        //axios.post('https://localhost:44356/validate/user',details)
        axios.post('https://localhost:7253/validate/user',details)
        .then((response) => {
            if(response.data.token) {
                localStorage.setItem('userToken',response.data.token);
            }
            console.log(response);
            setLoading(false);
            // setUserData(
            //     {...userData,
            //         UserType: response?.data?.data?.usertype,
            //         Name: response?.data?.data?.name,
            //         Email: response?.data?.data?.email
            //     }
            // )


            console.log(response);
            console.log(response?.data);
            console.log(response?.data?.data);
            let loggedInUserData = {...response?.data?.data};
            // this.props.setCurrentUserInfo({...loggedInUserData})
            console.log(loggedInUserData);
            dispatch(setCurrentUserInfo(loggedInUserData));
            dispatch(setAPIResponseCode(0));
            console.log(store.getState())
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
        history.push("/Home");
    }

    //console.log(this.props.currentUserInfo)
    console.log(store.getState());
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
                    <button onClick={ValidateUserAndSendOTP}>Send OTP</button>
                    {isOtpRequired && 
                    (
                        <div>
                            {keyString && <img src={`data:image/png;base64,${keyString}`} alt="MFA QR Code" width={200} height={200}/>}
                            <input type="text" value={otpValue} onChange={(e)=>{OnChangeOTPValue(e.target.value)}}></input>
                            {/* <button onClick={VerifyOTP}>Verify OTP</button> */}
                        </div>
                    )
                    }
                    <button onClick={VerifyOTP} disabled={check} className={`sign-up-btn ${check}`}>Login</button>
                    {/* <button onClick={handleUserSignUp} disabled={check} className={`sign-up-btn ${check}`}>Login</button> */}
                </div>
                </div>
            </div>
        </React.Fragment>
    )
}

export default LoginMFAQrCode;

// const mapStateToProps = (state = rootReducer) => ({
//     currentUserInfo: state.CurrentUserInfo,
// })
// const mapDispatchToProps = (dispatch) => {
//     setCurrentUserInfo = bindActionCreators(setCurrentUserInfo,dispatch)
// }

// export default connect(mapStateToProps,mapDispatchToProps)(LoginCopy);