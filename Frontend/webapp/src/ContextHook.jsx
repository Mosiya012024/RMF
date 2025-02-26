import { useState } from "react";
import React from "react";

export const ContextHook = React.createContext();

export const ContextProv = ({ children }) => {
    const [data, setData] = useState({
        name: 'Mosiya',
        ID: 376474,
        Department: 'CSE'
    });

    return (
        <ContextHook.Provider value={{data,setData}}>
            {children}
        </ContextHook.Provider>
    )
}