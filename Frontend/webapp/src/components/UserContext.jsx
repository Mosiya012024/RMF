import { React, useState, useEffect, createContext } from "react";

export const UserContext = createContext();

export const UserProvider = ({ children }) => {
    const details = {
      UserType: "",
      Name: "",
      Email : "",
    };
    
    // const [userData, setUserData] = useState(details);
    const [firstLogin,setFirstLogin] = useState(false);

    //storing the data of Context API in local Storage because whenever we reload the page the data stored in context api is lost as it is stored in browser's memory
    // but now using localStorage/sessionStorage to store the contents of Context API
    const [userData, setUserData] = useState(()=>{
        const savedState = sessionStorage.getItem('myState');
        return savedState ? JSON.parse(savedState) : details;
    })

    useEffect(()=>{
        sessionStorage.setItem('myState',JSON.stringify(userData));
    },[userData]);

    return (
        <UserContext.Provider value = {[[userData, setUserData],[firstLogin,setFirstLogin]]}>
            {children}
        </UserContext.Provider>
    )
}
