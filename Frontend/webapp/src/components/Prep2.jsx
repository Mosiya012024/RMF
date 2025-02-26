import { useEffect } from "react";
import React from "react";

function Prep2({handleClick,children}) {

    console.log("rendering the child",children);
    
    return(
        <div>
            <button onClick={handleClick}>{children}</button>
        </div>
    )
}

export default React.memo(Prep2);