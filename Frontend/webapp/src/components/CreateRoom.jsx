import React from "react";
import { Modal } from "@mui/material";
import './CreateRoom.css';
import axios from "axios";
import { toast } from "react-toastify";


class CreateRoom extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            userDetails: {},
            closeModal: false,
        }
    }
    render() {
        const details = ["Name","Area","City","State","Size","Vacancy","Gender","Message","Amount","Status"];
        let handleChange = (ans, item) => {
            this.setState({userDetails: {...this.state.userDetails,[item] : ans}})
        }
        let handleOnBlur = (ans,item) => {

        }
        let SaveUserData = ()=>{
            const base = {
                "name":this.state.userDetails.Name,
                "address": {
                    "area": this.state.userDetails.Area,
                    "city": this.state.userDetails.City,
                    "state":this.state.userDetails.State
                },
                "requirement": {
                    "size":this.state.userDetails.Size,
                    "vacancy": this.state.userDetails.Vacancy,
                    "gender": this.state.userDetails.Gender,
                    "message": this.state.userDetails.Message
                },
                "amount":this.state.userDetails.Amount,
                "status": this.state.userDetails.Status,
            }
            console.log(base);
            axios.post('https://localhost:44356/post/CreateRoom',base)
            .then((response) => {
                console.log(response)
                this.setState({userDetails: {}})
                this.props.handleClose();
                toast.success("Room added Sucessfully !",{
                    position: 'top-center',
                    autoClose: 30000
                });
            })
            .catch((error)=>{console.log(error)})            
        }
        console.log(this.state.userDetails);
        const handleCloseButton = () => {
            this.setState({userDetails:{}})
            this.props.handleClose();
        }

        const handleCancelButton = () => {
            this.setState({userDetails:{}})
            this.props.handleClose();
        }

        console.log(this.state.closeModal);

        let mainModal = (
            <div className="main-modal">
                <button className="modal-close-btn" onClick={handleCloseButton}>X</button>
                <div style={{color:'teal', fontFamily:'sans-serif', fontSize:'larger', textAlign:'center'}}>Post Room Details</div>
                <div className="modal-input-tags">
                    {details.map((element)=>{
                        return (<div className="input-ele-main">
                            <div>{element}</div>
                            <input type="text" placeholder={`Enter ${element}`} value={this.state.userDetails[element]} 
                            onBlur={(e)=>{handleOnBlur(e.target.value,element)}} 
                            onChange={(e)=>handleChange(e.target.value,element)}></input>
                            {}
                            </div>
                        )
                    })}    
                </div>
                <button className="cancel-btn" onClick={handleCancelButton}>Cancel</button>
                <button className="save-btn" onClick={SaveUserData} disabled="">Save</button>
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
                <div style={{width:'55%',margin:'90px auto 105px auto'}}>
                    {mainModal}
                </div>
                
            </Modal>
            </>
        )
    }
}
export default CreateRoom;