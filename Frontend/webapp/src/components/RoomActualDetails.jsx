import NavBar from "./NavBar";
import React from "react";
import axios from "axios";
import './RoomActualDetails.css';
import imageSlides from './CarosuelImages.json';
import DCCBreadcrumbs from "../shared/DCCBreadcrumbs";

class RoomActualDetails extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            RoomId: "",
            RoomDescription: {},
            slideNumber: 0,
            userIdentity: "",
            imageSlides:[],
        }
    }

    componentDidMount() {
        console.log(window.location.pathname);
        let url = window.location.pathname;
        let params = url.toString().split("/");
        console.log(decodeURIComponent(params[1]));
        this.setState({RoomId: params[2],userIdentity: decodeURIComponent(params[1])},
            () => {
                this.fetchRoomDescription();
            }
        );
    }

    fetchRoomDescription = () => {
        //axios.get(`https://localhost:44356/getRDX?filterId=${this.state.RoomId}`,{
        axios.get(`https://localhost:7253/getRDX?filterId=${this.state.RoomId}`,{
            headers:{
                'Authorization': `Bearer ${localStorage.getItem('userToken')}`
            }
        })
        .then((res)=>{
            console.log(res);
            this.setState({RoomDescription: res?.data?.data})
        })
        .catch((error)=>{
            console.log(error);
        })
        axios.get(`https://localhost:7253/get/room/images?folderId=${this.state.RoomId}`,
            {
                headers : {
                    'Authorization':  `Bearer ${localStorage.getItem('userToken')}`
                }
            }
        )
        .then((response)=>{
            this.setState({imageSlides: [...response?.data?.data]});
        })
        .catch((error) => console.log(error));
    }

    prevSlide = () => {
        this.setState({slideNumber: this.state.slideNumber-1})
    }
    nextSlide = () => {
        this.setState({slideNumber: this.state.slideNumber+1})
    }

    setSlide = (idx) => {
        this.setState({slideNumber: idx})
    }
    render() {
        console.log(this.state.imageSlides);
        console.log(this.state.RoomDescription);
        const availableDate = new Date(this.state.RoomDescription.availableFrom);
        const formattedAvailableDate = availableDate.toLocaleDateString('en-GB');
        console.log(formattedAvailableDate);
        return(<>
            {this.state.userIdentity === "Room Mate Finder" ?
            (<div className="main-room-description">
                <DCCBreadcrumbs
                        parts = {['Home','/places',this.state.userIdentity,localStorage.getItem('clickedListing')]}
                />
                <h1 style={{color: 'rgb(30,50,85)'}}>
                    {this.state.RoomDescription.gender} required in {this.state.RoomDescription.size} apartment in {this.state.RoomDescription.state}
                </h1>
                <p style={{fontSize:'smaller'}}>{this.state.RoomDescription.flatNumber}</p>
                <div className="carousel">
                    <button className="arrow left-arrow" onClick={()=>this.prevSlide()} disabled={this.state.slideNumber === 0}>&lt;</button>
                    {this.state.imageSlides.map((image,index)=>{
                        console.log(image);
                        return (
                            <img src={`data:${image.mimeType};base64,${image.base64StringData}`}
                             alt = {index}
                             height={350}
                             width={650}
                             className={this.state.slideNumber===index ? "slide" : "slide slide-hidden"}
                            />
                        )
                    })}
                    <button className="arrow right-arrow" onClick={()=>this.nextSlide()} disabled={this.state.slideNumber  === imageSlides.slides.length-1}>&gt;</button>
                    <span className="indicators">
                        {imageSlides.slides.map((_, idx) => {
                        return (
                            <button
                            key={idx}
                            className={
                                this.state.slideNumber === idx ? "indicator" : "indicator indicator-inactive"
                            }
                            onClick={()=>this.setSlide(idx)}
                            ></button>
                        );
                        })}
                    </span>
                </div>
                <div className="room-description-section">
                    <div className="room-description-section1">
                        <div style={{ color:'grey',marginBottom:'3px' }}>Type: {this.state.RoomDescription.amenityType}</div>
                        <div style={{color:'#624b4b'}}>Amenties: 
                            <span>
                                {this.state.RoomDescription.amenities?.map((item)=>{
                                    return (<span>{item},</span>)
                                })}
                            </span>
                        </div>
                        <h3>Flatmate  |  {this.state.RoomDescription.size}  |</h3>
                        <div style={{color:'#d1caca'}}>________________________</div>
                        <br></br>
                        <div style={{color:'#050567',fontSize:'18px',}}>{this.state.RoomDescription.amenityType}</div>
                        {(this.state.RoomDescription.amenityType === "Fully Furnished" || this.state.RoomDescription.amenityType === "Semi Furnished") && 
                            (<div className="Amenities">
                                {this.state.RoomDescription.amenities?.map((item)=>{
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
                                        <div className="amenity-item">
                                            <img src={imgSrc} alt="Fan" width={30} height={30}></img>
                                            <span style={{alignSelf: 'center',paddingLeft: '15px'}}>{item}</span>
                                        </div>
                                    )
                                })}
                            </div>
                        )}
                    </div>
                    <div className="room-description-section2">
                        <div><span>Rent :{this.state.RoomDescription.rent}</span></div>
                        <div><span>Deposit : {this.state.RoomDescription.deposit}</span></div>
                        <div><span>Maintenance : {this.state.RoomDescription.maintenance}</span></div>
                        <h4>Available From: {new Date(this.state.RoomDescription.availableFrom).toLocaleDateString('en-GB')}</h4>
                        <h5>Posted on: {new Date(this.state.RoomDescription.postedOn).toLocaleDateString('en-GB')}</h5>
                    </div> 
                </div> 
                
            </div>):
            (
                <div className="main-room-description">
                    <DCCBreadcrumbs
                        parts = {['Home','/places',this.state.userIdentity,localStorage.getItem('clickedListing')]}
                    />
                    <div className="room-finder-section">
                        <div className="photo-section">

                        </div>
                        <div className="room-finder-details-section">
                            <div>
                                <div className="location-text space-down" >Location</div>
                                <div className="occupancy-text">{this.state.RoomDescription.flatNumber}</div>
                            </div>
                            <div className="hrLine"></div>
                            <div>
                                <div className="location-text">Basic Info</div>
                                <div className="basic-info">
                                    <div>
                                        <div className="occupancy-text space-down">Occupancy</div>
                                        <div className="location-text">{this.state.RoomDescription.size}</div>
                                    </div>
                                    <div>
                                        <div className="occupancy-text space-down">Looking for</div>
                                        <div className="location-text">{this.state.RoomDescription.gender}</div>
                                    </div>
                                    <div>
                                        <div className="occupancy-text space-down">Approx Rent</div>
                                        <div className="location-text">{this.state.RoomDescription.rent}</div>
                                    </div>
                                </div>
                            </div>
                            <div className="hrLine"></div>
                            <div className="room-finder-description-section">
                    <div className="room-description-section1">
                        <div style={{ color:'grey',marginBottom:'3px' }}>Type: {this.state.RoomDescription.amenityType}</div>
                        <div style={{color:'#624b4b'}}>Amenties: 
                            <span>
                                {this.state.RoomDescription.amenities?.map((item)=>{
                                    return (<span>{item},</span>)
                                })}
                            </span>
                        </div>
                        <h3>Roommate  |  {this.state.RoomDescription.size}  |</h3>
                        <div style={{color:'#d1caca'}}>________________________</div>
                        <br></br>
                        <div style={{color:'#050567',fontSize:'18px',}}>{this.state.RoomDescription.amenityType}</div>
                        {(this.state.RoomDescription.amenityType === "Fully Furnished" || this.state.RoomDescription.amenityType === "Semi Furnished") && 
                            (<div className="Amenities">
                                {this.state.RoomDescription.amenities?.map((item)=>{
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
                                        <div className="amenity-item">
                                            <img src={imgSrc} alt="Fan" width={30} height={30}></img>
                                            <span style={{alignSelf: 'center',paddingLeft: '15px'}}>{item}</span>
                                        </div>
                                    )
                                })}
                            </div>
                        )}
                    </div>
                    <div className="room-description-section2">
                        <div><span>Rent :{this.state.RoomDescription.rent}</span></div>
                        <div><span>Deposit : {this.state.RoomDescription.deposit}</span></div>
                        <div><span>Maintenance : {this.state.RoomDescription.maintenance}</span></div>
                        <h4>Available From: {new Date(this.state.RoomDescription.availableFrom).toLocaleDateString('en-GB')}</h4>
                        <h5>Posted on: {new Date(this.state.RoomDescription.postedOn).toLocaleDateString('en-GB')}</h5>
                    </div> 
                </div> 
                            
                        </div>
                    </div>
                </div>
            )
            }
            </>
        )
    }
}
export default RoomActualDetails;