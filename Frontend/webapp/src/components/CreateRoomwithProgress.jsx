import React from "react";
import { Modal } from "@mui/material";
import "./CreateRoom.css";
import "./CreateRoomwithProgress.css";

class CreateRoomwithProgress extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            progress: 0,
        }
    }

    handlePrevClick = () => {
        if(this.state.progress>0) {
            this.setState({progress: this.state.progress-20});
        }
    }
    handleNextClick = () => {
        if(this.state.progress<100) {
            this.setState({progress: this.state.progress+20});
        }
    }
    render() {

        let mainModal = (
            <div className="main-modal">
                <br>
                </br>
                <br></br>
                <div className="progressBar">
                    <div className="progressBarFill" style={{width: `${this.state.progress}%`}}></div>
                </div>
                <div className="progressLabel">{this.state.progress}</div>
                    <button onClick={this.handlePrevClick} disabled={this.state.progress === 0}>Prev</button>
                    <button onClick={this.handleNextClick}disabled={this.state.progress === 100}>Next</button>
            </div>
        )
        return (
            <>
            <Modal
                open={this.props.open}
                onClose={this.props.handleClose}
                aria-labelledby="simple-modal-title"
                aria-describedby="simple-modal-description"
            >
                <div style={{width:'55%',margin:'80px auto 80px auto'}}>
                    {mainModal}
                </div>
            </Modal>
            </>
        )
    }
}

export default CreateRoomwithProgress;