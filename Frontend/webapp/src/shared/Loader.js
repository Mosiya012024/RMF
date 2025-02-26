import React from "react";
import "./Loader.css"
function Loader(props) {
    return (
        <React.Fragment>
            {props.isLoading && 
                <div className="overlay"> 
                    <div className="loader"></div>
                </div>
            }
        </React.Fragment>
       
    )
}
export default Loader;