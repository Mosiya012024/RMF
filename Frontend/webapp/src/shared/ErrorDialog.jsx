import React, { useState } from "react";
import "./ErrorDialog.css";
import errorImage from "../images/errorImage.png"
import { Dialog } from "@mui/material";

const ErrorDialog =(props) => {
    const [open, setOpen] = useState(props.open);

    const handleOkButtonClick =() => {
        setOpen(false);
        props.handleDialogOk();
    }
    console.log(props.open);
    return (
        <>
        <Dialog
            open = {open}
            className={""}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
            fullWidth={false}
            maxWidth="md"
            onClose={handleOkButtonClick}
        >
            <div className="error-dialogbox">
                <div className="error-title">{props.ErrorTitle}</div>
                <br/>
                <div className="error-description">
                    <img src={errorImage} width={50} height={50} style={{marginRight:'10px'}} alt="empty"></img>
                    <div>{props.ErrorTxt}</div>
                </div>
                <button className={"error-button-primary"}onClick={handleOkButtonClick}>OK</button>
            </div>
        </Dialog>
        
        </>
    )
}
export default ErrorDialog;