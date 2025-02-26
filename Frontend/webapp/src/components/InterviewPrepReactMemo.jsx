import { useEffect, useState } from "react";
import { memo } from "react";

function InterviewPrepReactMemo() {
    const [name, setName] = useState("Mosiya");

    useEffect(()=>{
        setInterval(()=>{
            console.log("executing interval");
            setName("Mosiyaaa");
        },[5000])
    })

    console.log("Data is changed");
    return(
        <div>
            {name}
        </div>
    )
}
export default memo(InterviewPrepReactMemo);