import React, { useEffect, useState } from "react";
import  './Home.css';
import RMFLogo from "../images/roommatefinder_logo_final.png";
import { useHistory } from "react-router-dom";

function Home() {
    const history = useHistory();
    const [selectedPlace,setSelectedPlace] = useState("");
    let debounceTimeOut = null;
    const handleLoginClick =  () => {
        console.log("handle login click");
        history.push("/log-in");
    }

    const handleSignUpClick = () => {
        console.log("handle signup click");
        history.push("/sign-up");
    }

    const navigateToPlaces = (place) => {
        console.log(place);
        setSelectedPlace(place);
        // if(place.length == 5) {
        //     history.push(`/places?location=${place}`);
        // }
        // setTimeout(()=>{
        //     history.push(`/places?location=${place}`);
        // },5000)
    }

    useEffect(() => {
        const handler = setTimeout(() => {
          if (selectedPlace) {
            // Perform your search or action here
            history.push(`/places?location=${selectedPlace}`);
            console.log('Search for:', selectedPlace);
          }
        }, 10000); // 500ms debounce time
    
        return () => clearTimeout(handler);
      }, [selectedPlace]);

    // useEffect(()=>{
    //     if(debounceTimeOut) {
    //         clearTimeout(debounceTimeOut);
    //     }

    //     debounceTimeOut = setTimeout(()=>{
    //         history.push(`/places?location=${selectedPlace}`);
    //     },5000);
    // },[selectedPlace])

    return (
        <React.Fragment>
        <div className="nav-bar"> 
            <div className="app-logo">
                <img src = {RMFLogo} alt="Room Mate Finder logo" width={200} height={100}></img>
            </div>
            <div className="login-btn" onClick={handleLoginClick}>Login</div>
            <div className="signup-btn" onClick={handleSignUpClick}>SignUp</div>
        </div>
        <div className="other-rows">
            <span className="trustedText">Trusted and loved by million users</span>
            <h1 className="findText">Find Compatible flatmates  Rooms & PGs</h1>
            <div style={{fontFamily:'cursive',color:'#949494'}}>100000+ Rooms And Flatmates Available Now Across</div>
            <input
                className="searchPlaces"
                type="search"
                placeholder="Search places..."
                value={selectedPlace}
                onChange={(e)=>navigateToPlaces(e.target.value)}
            />
        </div>
        
        </React.Fragment>
    )
}
export default Home;