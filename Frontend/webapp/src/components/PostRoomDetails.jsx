import React from "react";
import axios from "axios";
import { toast } from "react-toastify";
import { Modal } from "@mui/material";
import "./PostRoomDetails.css";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import TextField from "@mui/material/TextField";
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import dayjs from 'dayjs';
import { DemoContainer } from '@mui/x-date-pickers/internals/demo';
import Box from '@mui/material/Box';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import Select from '@mui/material/Select';
import { connect } from "react-redux";
import Stack from "@mui/material/Stack";
import PhotoCamera from "@mui/icons-material/PhotoCamera";
import IconButton from "@mui/material/IconButton";
import Input from "@mui/material/Input";
import { AttachFile } from "@mui/icons-material/AttachFile";
import { Padding } from "@mui/icons-material";
import { InitializeSignalR } from "../shared/NotificationService";
import { rootReducer } from "../redux/store/store";

class PostRoomDetails extends React.Component {
    constructor(props) {
        
        super(props);
        
        this.state = {
            progress: 0,
            pageNumber: 0,
            userDetails: {
                vacancy: 0,
                state: "",
                amenitiesList : [],
                availableFrom: dayjs(),
            },
            disableNextBtn: false,
            disablePrevBtn:false,
            uploadedFiles: [],
            uploadMessage: "",
            userEditingImages: false,
        }
    }

    // componentDidUpdate = () => {
    //     if(this.state.userDetails.identity !== null && this.state.userDetails.identity !== undefined && this.state.userDetails.identity !== ""){
    //         InitializeSignalR();
    //     }
    // }
    handlePrevClick = () => {
        if(this.state.progress>0) {
            this.setState({progress: this.state.progress-16.66,pageNumber: this.state.pageNumber-1});
        }
    }
    handleCloseModal = () => {
        this.setState({
            pageNumber: 0,
            userDetails: {
                vacancy: 0,
                state: "",
                amenitiesList : [],
                availableFrom: dayjs(),
            },
            progress: 0,
        });
        this.props.handleClose();

    }

    generateGuid = () => {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
          const r = (Math.random() * 16) | 0;
          const v = c === 'x' ? r : (r & 0x3) | 0x8;
          return v.toString(16);
        });
    };

    handleNextClick = (btnType) => {
        if(Math.round(this.state.progress)<100) {
            this.setState({progress: this.state.progress+16.66,pageNumber: this.state.pageNumber+1});
        }
        if(btnType === "Submit") {
            const base = {
                "id": (this.state.userDetails?.id === null || this.state.userDetails?.id === undefined || this.state.userDetails?.id === "") ? this.generateGuid() : this.state.userDetails?.id,
                "name":this.state.userDetails.name,
                "roomType" :this.state.userDetails.roomType,
                "address": {
                    "area": this.state.userDetails.area,
                    "city": this.state.userDetails.city,
                    "state":this.state.userDetails.state,
                },
                "requirement": {
                    "size":this.state.userDetails.size,
                    "vacancy": this.state.userDetails.vacancy,
                    "gender": this.state.userDetails.gender,
                    "message": "Welcome to RoomMateFinder",
                },
                "amount": this.state.userDetails.rent,
                "deposit": this.state.userDetails.deposit,
                "maintenance": this.state.userDetails.maintenance,
                "amenityType": this.state.userDetails.amenityType,
                "amenities": this.state.userDetails.amenitiesList, 
                "status": this.state.userDetails.vacancy > 0 ? "Vacant" : "Occupied",
                "flatNumber": this.state.userDetails.flatNumber ?? "",
                "availableFrom": this.state.userDetails.availableFrom,
            }
            console.log(base);
            if(this.props.editingData) {
                //axios.put('https://localhost:44356/edit/roomCombinedDetails',base,{
                axios.put('https://localhost:7253/edit/roomCombinedDetails',base,{
                    headers:{
                        'Authorization': `Bearer ${localStorage.getItem('userToken')}`
                    }
                })
                .then((response) => {
                    console.log(response)
                    this.setState({userDetails: {
                        vacancy: 0,
                        state: "",
                        amenitiesList : []
                    }})
                    this.props.handleClose();
                    toast.success(`${this.state.userDetails.identity}`+" edited Sucessfully !",{
                        position: 'top-center',
                        autoClose: 30000
                    });
                })
                .catch((error)=>{console.log(error)})  
            }
            else {
                //axios.post('https://localhost:44356/post/CreateRoomCombinedDetails',base,{
                axios.post('https://localhost:7253/post/CreateRoomCombinedDetails',base,{
                    headers:{
                        'Authorization': `Bearer ${localStorage.getItem('userToken')}`
                    }
                })
                .then((response) => {
                    console.log(response)
                    this.setState({userDetails: {
                        vacancy: 0,
                        state: "",
                        amenitiesList : []
                    }})
                    this.props.handleClose();
                    let userIdentityCreate = this.state.userDetails.roomType === 'Room mate' ? 'Room Finder' : 'Room Mate Finder';
                    toast.success(`${userIdentityCreate}`+" added Sucessfully !",{
                        position: 'top-center',
                        autoClose: 30000
                    });
                })
                .catch((error)=>{console.log(error)})
            }
                
        }
        
    }

    handleCurrentButtonClick = (type, item) => {
        if(type === "availableFrom") {
            this.setState({userDetails: {...this.state.userDetails ,[type]: item}});
        }
        //this.setState({roomType: item});
        if(type === "roomType" || type === "size" || type === "gender" || type === "amenityType") {
            if(this.state.userDetails[type] === "") {
                this.setState({userDetails: {...this.state.userDetails ,[type]: item}});
            }
            else if(this.state.userDetails[type] === item) {
                this.setState({userDetails: {...this.state.userDetails ,[type]: ""}});
            } 
            else {
                this.setState({userDetails: {...this.state.userDetails ,[type]: item}});
            }
        }
        if(type === "vacancy") {
            if(item === "Add") {
                this.setState({userDetails: {...this.state.userDetails ,[type]: this.state.userDetails[type]+1}});
            }
            if(item === "Sub") {
                this.setState({userDetails: {...this.state.userDetails ,[type]: this.state.userDetails[type]-1}});
            }
        }
        if(type === "rent" || type === "deposit" || type === "maintenance" || type === "state" || type==="name" || type === "area" || type ==="city" || type === "flatNumber") {
            this.setState({userDetails: {...this.state.userDetails ,[type]: item}});
        }
        if(type === "amenitiesList") {
            if(this.state.userDetails[type]?.includes(item)) {
                let arr = this.state.userDetails[type];
                let idx = arr.indexOf(item);
                arr.splice(idx,1);
                this.setState({userDetails: {...this.state.userDetails, [type]: [...arr]}});
            }
            else {
                this.setState({userDetails: {...this.state.userDetails, [type]: [...this.state.userDetails[type],item]}});
            }
        }
    }

    disablePrevious = () => {
        if(this.state.progress === 0) {
            return true;
        }
        else {
            return false;
        }
    }

    disableNext = () => {
        let keys = Object.keys(this.state.userDetails);
        for(let i=0;i<keys.length;i++) {
            if(keys[i] !== "state" && this.state.userDetails[keys[i]] === "") {
                this.setState({disableNextBtn: true});
                return this.state.disableNextBtn;
            }
        }
        return this.state.disableNextBtn;
            
    }

    setEditData = () => {
        console.log(this.props.roomCombinedDetails)
        if((this.props?.roomCombinedDetails !== undefined && this.props?.roomCombinedDetails !== null && Object.keys(this.props?.roomCombinedDetails)?.length !== 0
            && this.props?.editingData === true && Object.keys(this.state.userDetails)?.length === 4)) {
            this.setState({userDetails: {...this.props.roomCombinedDetails}},()=>{
                axios.get(`https://localhost:7253/get/room/images?folderId=${this.state.userDetails.id}`,
                    {
                        headers : {
                            'Authorization':  `Bearer ${localStorage.getItem('userToken')}`
                        }
                    }
                )
                .then((response)=>{


                    let finalData = response?.data?.data.map((element)=> {
                        if(element.mimeType) {
                            const byteCharacters = atob(element.base64StringData); // Decode base64
                            const byteNumbers = new Array(byteCharacters.length);
        
                            for (let i = 0; i < byteCharacters.length; i++) {
                                byteNumbers[i] = byteCharacters.charCodeAt(i);
                            }
        
                            const byteArray = new Uint8Array(byteNumbers);
                            const blob = new Blob([byteArray], { type: element.mimeType });
                           
                            // Convert Blob to File
                            return new File([blob], element.imageName.split("/")[1].replace("_.jpg","").replace("_.jpeg",""),{ type: element.mimeType });
                        }
                        else {
                            return element;
                        }
                    })
                    console.log(finalData);
                    this.setState({uploadedFiles: [...finalData]});
                })
                .catch((error) => console.log(error));
            });
        }
    }

    handleFileChange = (selectedFile) => {

        console.log(selectedFile[0].name);
        let checkAlreadyExistingFile = this.state.uploadedFiles.every((x)=>
            x.name !== selectedFile[0].name
        );
        console.log(checkAlreadyExistingFile);
        if(checkAlreadyExistingFile) {
            const presentFiles = this.state.uploadedFiles;
            this.setState({uploadedFiles: [...presentFiles,...selectedFile],uploadMessage:"",userEditingImages: this.props.editingData})
        }
       
        
    }

    handleFileUpload = async () => {
        console.log(this.state.uploadedFiles);
        if (!this.state.uploadedFiles) {
            this.setState({uploadMessage: "Please select a file first"});
            return;
        }

            
            // this.state.uploadedFiles.map((element)=> {
            //     if(!element?.mimeType) {
            //         const byteCharacters = atob(element.base64StringData); // Decode base64
            //         const byteNumbers = new Array(byteCharacters.length);

            //         for (let i = 0; i < byteCharacters.length; i++) {
            //             byteNumbers[i] = byteCharacters.charCodeAt(i);
            //         }

            //         const byteArray = new Uint8Array(byteNumbers);
            //         const blob = new Blob([byteArray], { type: element.mimeType });
            //         console.log(element.imageName.split("/")[1].replace("_.jpg",""))
            //         // Convert Blob to File
            //         return new File([blob], element.imageName.split("/")[1].replace("_.jpg",""),{ type: element.mimeType });
            //     }
            //     else {
            //         return element;
            //     }
            // })
            let formData = new FormData();
                this.state.uploadedFiles.forEach(element => {
                    formData.append("file", element);
            });
          
          try {
            let generateId = this.props.editingData ? this.state.userDetails?.id : this.generateGuid()
            const response = await axios.post(
              `https://localhost:7253/upload/room/images?folderId=${generateId}`, 
              formData,
              {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('userToken')}`,
                    "Content-Type": "multipart/form-data",
                },
              }
            );
            this.setState({uploadMessage: "files uploaded successfully", userDetails:{...this.state.userDetails,id: generateId}});
            //setUploadMessage(`Upload successful: ${response.data.BlobUrl}`);
          } catch (error) {
            this.setState({uploadMessage: "files are not updated.Please try again",uploadedFiles:[]});
            //setUploadMessage("Upload failed: " + error.message);
          }
    }

    handleDeleteFile = (indexToDelete) => {
        this.setState({uploadedFiles: this.state.uploadedFiles.filter((_,index)=> index !== indexToDelete)});
        //this.setState((uploadedFiles) => uploadedFiles.filter((_, index) => index !== indexToDelete));
    }

    render() {
        console.log(this.state.uploadedFiles);
        console.log(this.props.roomCombinedDetails)
        console.log(this.state.userDetails);
        let roomType = ["Room mate","Room","PG"];
        let allSizes = this.state.userDetails["roomType"] === "Room" ? ["1BHK","2BHK","3BHK","4BHK","5BHK","5+BHK"] : ["Single", "2 - Shared","3 - Shared","4 - Shared"];
        let allGenders = ["Male","Female","Any"];
        let allAmenityTypes = ["Fully Furnished", "Semi Furnished","Un Furnished"];
        let amenities = ["Fan","AC","Refrigerator","TV","WashingMachine","Sofa"];
        let allStates = ["Andhra Pradesh","Telangana","Maharastra","Karnataka","Kerala","Tamil Nadu","Delhi","Jammu and Kashmir"];
        let firstPage = (
            <>
                <div>Advertise Roommates, Rooms, PGs</div><br></br>
                <ButtonGroup
                    orientation="vertical"
                    aria-label="Vertical button group"
                >
                    {roomType.map((item) => {
                        return (<Button style={(this.state.userDetails["roomType"] === item && this.state.userDetails["roomType"] !== "")? {backgroundColor: '#90abe7'} : {}} key={item} onClick={()=>{this.handleCurrentButtonClick("roomType",item)}}>{item}</Button>)
                    })}
                </ButtonGroup>
            </>
        )
        let secondPage = (
            <>
            <div>Select Room Model</div>
            <div className="allSizes">
                {allSizes.map((item) => {
                    return (<Button  variant="outlined" style={(this.state.userDetails["size"] === item && this.state.userDetails["size"] !== "")? {backgroundColor: '#90abe7'} : {}} key={item} onClick={()=>{this.handleCurrentButtonClick("size",item)}}>{item}</Button>)
                })}
            </div>
            {
                this.state.userDetails["roomType"] === "Room" ? 
                (<TextField style={{width: '400px'}}id="outlined-basic" label="FlatNumber" variant="outlined" onChange={(e)=>{this.handleCurrentButtonClick("flatNumber",e.target.value)}} value={this.state.userDetails["flatNumber"]}/>) : (<></>)
            }
            </>
        )
        let thirdPage = (
            <>
            <div>Looking for </div><br></br>
            <ButtonGroup
                    orientation="vertical"
                    aria-label="Vertical button group"
                >
            {allGenders.map((item) => {
                    return (<Button style={(this.state.userDetails["gender"] === item && this.state.userDetails["gender"] !== "")? {backgroundColor: '#90abe7'} : {}} key={item} onClick={()=>{this.handleCurrentButtonClick("gender",item)}}>{item}</Button>)
                })}
            </ButtonGroup>
            
            {
                this.state.userDetails["roomType"] === "Room" ? 
                (
                    <>
                        <div>Number of Roommates/ Flatmates Required</div><br></br>
                        <ButtonGroup variant="outlined" aria-label="Basic button group">
                            <Button key={"-"} disabled = {this.state.userDetails["vacancy"]===0} onClick={()=>{this.handleCurrentButtonClick("vacancy","Sub")}}>-</Button>
                            <Button  key={''} >{this.state.userDetails["vacancy"]}</Button>
                            <Button key={"+"} onClick={()=>{this.handleCurrentButtonClick("vacancy","Add")}}>+</Button>
                        </ButtonGroup>
                    </>
                )
                : (<></>)
            }
            
            </>
        )
        let fourthPage = (
            <>
            <div>Enter Basic Details</div> <br></br>
            <TextField id="outlined-basic" label="Rent" variant="outlined"  style={{marginBottom:'10px'}} onChange={(e)=>{this.handleCurrentButtonClick("rent",e.target.value)}} value={this.state.userDetails["rent"]}/>
            <TextField id="outlined-basic" label="Deposit" variant="outlined" style={{marginBottom:'10px'}} onChange={(e)=>{this.handleCurrentButtonClick("deposit",e.target.value)}} value={this.state.userDetails["deposit"]}/>
            <TextField id="outlined-basic" label="Maintenance" variant="outlined" onChange={(e)=>{this.handleCurrentButtonClick("maintenance",e.target.value)}} value={this.state.userDetails["maintenance"]}/>
            </>
        )
        let fifthPage = (
            <>
            <div>Select Type</div>
            <div className="AmenityType"> 
                {
                    allAmenityTypes.map((item)=>{
                        return (<Button variant="outlined"
                                onClick={()=>{this.handleCurrentButtonClick("amenityType",item)}}
                                style={(this.state.userDetails["amenityType"] === item && this.state.userDetails["amenityType"] !== "")? {backgroundColor: '#90abe7'} : {}}
                                >{item}</Button>)
                    })
                }
            </div>
            <div>Amenities</div>
            <div className="AmenityType">
                {
                    amenities.map((item)=>{
                        let imgSrc=""
                                    if(item === "Fan") {
                                        imgSrc = "https://findmyroom.in/_next/image?url=%2Fimages%2Ffan.png&w=128&q=75";
                                    }
                                    if(item === "AC") {
                                        imgSrc = "https://findmyroom.in/_next/image?url=%2Fimages%2Fac.png&w=128&q=75";
                                    }
                                    if(item === "Refrigerator") {
                                        imgSrc = "https://findmyroom.in/_next/image?url=%2Fimages%2Frefrigerator.png&w=128&q=75";
                                    }
                                    if(item === "Sofa") {
                                        imgSrc = "https://findmyroom.in/_next/image?url=%2Fimages%2Fsofa.png&w=128&q=75";
                                    }
                                    if(item === "TV") {
                                        imgSrc = "https://findmyroom.in/_next/image?url=%2Fimages%2Ftv.png&w=128&q=75";
                                    }
                                    if(item === "WashingMachine") {
                                        imgSrc = "https://findmyroom.in/_next/image?url=%2Fimages%2FwashingMachine.png&w=128&q=75";
                                    }
                        return (
                            <Button 
                            style={(this.state.userDetails["amenitiesList"]?.includes(item))? {backgroundColor: '#b9bdc0'} : {}}
                            onClick={()=>{this.handleCurrentButtonClick("amenitiesList",item)}}><img src={imgSrc} alt={item} width={60} height={60}></img></Button>
                        )
                    })
                }
            </div>
            
            </>
        )

        let SixthPage = (
            <>
            <div>Enter Address </div>
            <div className="Address-modal">
            <TextField id="outlined-basic" label="Name of poster" variant="outlined" onChange={(e)=>{this.handleCurrentButtonClick("name",this.props.currentUserInfo.CurrentUserInfo.identity)}} value={this.state.userDetails["name"]} disabled/>
            {/* <LocalizationProvider dateAdapter={AdapterDayjs}>
                <DemoContainer components={['DatePicker']}>
                    <DatePicker
                        label="Controlled picker"
                        // value={value}
                        // onChange={(newValue) => setValue(newValue)}
                    />
                </DemoContainer>
            </LocalizationProvider> */}
            <Box sx={{ minWidth: 210  }}>
                <FormControl fullWidth>
                    <InputLabel id="demo-simple-select-label">State</InputLabel>
                    <Select
                    labelId="demo-simple-select-label"
                    id="demo-simple-select"
                    value={this.state.userDetails["state"]}
                    label={"State"}
                    onChange={(e)=>{this.handleCurrentButtonClick("state",e.target.value)}}
                    >
                        {
                            allStates.map((item)=> {
                                return (
                                <MenuItem value={item}>{item}</MenuItem>)
                            })
                        }
                    </Select>
                </FormControl>
            </Box>
            <TextField id="outlined-basic" label="City" variant="outlined" onChange={(e)=>{this.handleCurrentButtonClick("city",e.target.value)}} value={this.state.userDetails["city"]}/>
            <TextField id="outlined-basic" label="Area" variant="outlined" onChange={(e)=>{this.handleCurrentButtonClick("area",e.target.value)}} value={this.state.userDetails["area"]}/>
            </div>
            </>
        )
        
        let SeventhPage = (
            <>
                <LocalizationProvider
                    dateAdapter={AdapterDayjs}   
                    adapterLocale={"en"}
                    >
                        <DatePicker
                            value={this.state.userDetails.availableFrom}
                            label={"Available From"}
                            onChange={(newValue)=>this.handleCurrentButtonClick("availableFrom",newValue)}
                        />
                        </LocalizationProvider>
            </>
        )

        let fileUploadPage = (
            <>
                <div className="btn-group">
                    <Button variant="contained" component="label">
                        Browse File
                        <input type="file" hidden onChange={(e)=>this.handleFileChange(e.target.files)}/>
                    </Button>
                    <Button variant="contained" component="label" onClick={this.handleFileUpload}>
                    Upload File
                    </Button>
                </div>
                <ul className="allfiles">
                    {this.state.uploadedFiles.map((file,index)=>{
                        console.log(file);
                        return (
                            <li key={index} style={{listStyle:'none', margin:'10px'}}>
                                <div style={{padding:'10px', border: '1px solid grey', borderRadius:'10px',display:'flex'}}>
                                    <img src={(file?.base64StringData) ? `data:${file.mimeType};base64,${file.base64StringData}` : URL.createObjectURL(file)} alt={file.name} width={50} height={50}></img>
                                    <button className="btn-x" onClick={()=>this.handleDeleteFile(index)}>x</button>
                                </div>
                            </li>
                        )
                    })}
                </ul>
                <div>{this.state.uploadMessage}</div>
            </>
        )
        
        let allPages;
        if(this.state.userDetails["roomType"] === "Room") {
            allPages = [firstPage,secondPage,thirdPage,fourthPage,fileUploadPage,fifthPage,SixthPage,SeventhPage];
        }
        else {
            allPages = [firstPage,secondPage,thirdPage,fourthPage,fifthPage,SixthPage,SeventhPage];
        }
        
        let mainModal = (
            <div className="main-modal" onClick={()=>this.setEditData()}>
                <br>
                </br>
                <div className="room-heading">{this.props.roomCombinedDetails ? this.props.roomCombinedDetails.identity === "Room Mate Finder" ? "Edit Room Details":"Edit Room Mate Preferences":"Add Listing"}</div>
                <div className="progressBar">
                    <div className="progressBarFill" style={{width: `${this.state.progress}%`}}></div>
                </div>
                <div style={{marginLeft:'40px'}}>{Math.round(this.state.progress)}%</div>
                <br>
                </br>
                {allPages.map((page,idx) => {
                    return(<div className={this.state.pageNumber === idx ? "page" : "page slide-hidden"}>{page}</div>)
                })}
                <br></br>
                <div className="btnGroup">
                <Button variant="contained" color="success" disabled={Math.round(this.state.progress)===0} onClick={this.handlePrevClick}>Previous</Button>
                <Button variant="contained" color="success" disabled={this.state.progress > 100} onClick={()=>this.handleNextClick(this.state.progress >= 99.96 ? "Submit" : "Next")}>{this.state.progress >= 99.96 ? "Submit" : "Next"}</Button>
                </div>
            </div>
        )
        return (
            <>
            <Modal
                open={this.props.open}
                onClose={this.handleCloseModal}
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

const mapStateToProps = (state = rootReducer) => ({
    currentUserInfo : state.CurrentUserInfo
})

export default connect(mapStateToProps)(PostRoomDetails);