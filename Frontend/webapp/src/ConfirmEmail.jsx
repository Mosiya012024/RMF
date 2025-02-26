import { useEffect, useState } from "react";
import axios from "axios";
import { useParams } from "react-router-dom/cjs/react-router-dom";

export default function ConfirmEmail() {
    const token = useParams();
    const [userData, setUserData] = useState({});
    console.log(token);
    useEffect(()=>{
        axios.get(`https://localhost:7253/get/confirm-mail?token=${token.tokenID}`)
        .then((response) => {
            console.log(response?.data?.data);
            setUserData(response?.data?.data);
        })
        .catch((error)=>console.log(error))
    },[token])

    return(
        <div className="gap"> 
            <div>
                Hii {userData.name}, you have signed up successfully with email ID {userData.email}.
            </div>
            
        </div>
    )
}