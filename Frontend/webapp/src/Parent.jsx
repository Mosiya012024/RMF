import { useContext, useState } from "react"
import Child from "./Child";
import { ContextHook } from "./ContextHook";


function Parent() {
    const [name, setName] = useState('Mosiya');
    const {contextData, setContextData} = useContext(ContextHook);

    console.log(contextData);
    return(
        <div>
            <div>{name}</div>
            <div>{contextData.name}</div>
            <div>{contextData.ID}</div>
            <div>{contextData.Department}</div>
            <Child userName={name}></Child>
        </div>
    )
}

export default Parent