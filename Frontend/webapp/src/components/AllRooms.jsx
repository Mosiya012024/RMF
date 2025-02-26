import { Component, createRef } from "react";
import React from "react";
import axios from "axios";
import './AllRooms.css';
import userLogo from "../images/user-logo.png"
import Loader from "../shared/Loader";
import { withRouter } from "react-router-dom";

class AllRooms extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data : [],
            loading: false,
            totalCount: 0,
            offset: 0,
            limit: 6,
            openGenderDropdown: false,
            gender: "Gender",
            location: "Select location",
            openLocationDropdown: false,
            totalData: [],
            showLocationSuggestion: false,
            searchByLocation : "",
            locationSuggestionList: [],
        }
        this.locationDropdownRef = createRef();
        this.genderDropdownRef = createRef();
        this.locationSuggestionDropdownRef = createRef();
    }

    fetchData() {
        let genderFilter = (this.state.gender !== "Gender") ? `gender eq ${this.state.gender}` : "";
        let locationFilter = (this.state.location !== "Select location") ? `location eq ${this.state.location}` : "";
        let data;
        if(genderFilter !== "" && locationFilter !== "") {
            data = genderFilter + " (and) " + locationFilter;
        }
        else {
            data = genderFilter+locationFilter;
        }
        
        axios.get(`https://localhost:44356/get/allRooms?offset=${this.state.offset}&limit=${this.state.limit}&filter=`+data)
        .then(
            (response) => {
                console.log(response?.data);
                console.log(response?.data?.data);
                this.setState({data:response?.data?.data, loading: false, totalCount: response?.data?.count});
            }
        )
    }

    componentDidUpdate(prevProps,prevState) {
        // console.log(prevState.gender);
        // console.log(this.state.gender);
        // if(this.state.gender !== prevState.gender) {
        //     this.fetchData();
        // }
        // this.fetchData();
        
    }

    componentDidMount() {
        this.fetchData();
        axios.get(`https://localhost:44356/get/allRooms?offset=0&limit=1000&filter=`)
        .then(
            (response) => {
                this.setState({totalData:response?.data?.data});
            }
        )
        document.addEventListener('mousedown', this.handleClickOutside);
    }

    componentWillUnmount() {
        document.addEventListener('mousedown', this.handleClickOutside);
    }

    handleClickOutside = (event) => {
        if(this.locationDropdownRef && !this.locationDropdownRef?.current?.contains(event.target)) {
            this.setState({openLocationDropdown: false});
        }
        if(this.genderDropdownRef && !this.genderDropdownRef?.current?.contains(event.target)) {
            this.setState({openGenderDropdown: false});
        }
        if(this.locationSuggestionDropdownRef && !this.locationSuggestionDropdownRef?.current?.contains(event.target)) {
            this.setState({showLocationSuggestion: false});
        }
        
    }

    handleNextBtn = () => {
        if(this.state.totalCount > this.state.offset+this.state.limit) {
            let limit = this.state.totalCount-(this.state.offset+this.state.limit)>=6 ? 6 : this.state.totalCount-(this.state.offset+this.state.limit);
            // this.setState(
            //     {
            //         offset: this.state.offset+this.state.limit,
            //         limit: limit,
            //     })
            // this.fetchData();
            this.setState(
                {
                    offset: this.state.offset+this.state.limit,
                    limit: limit,
                },
                () => {
                    this.fetchData();
                }
            );
        }
        // this.setState(
        //     {
        //         offset: this.state.offset+this.state.limit,
        //     },
        //     () => {
        //         this.fetchData();
        //     }
        // );
    }
    handlePrevBtn = () => {
        if(this.state.offset > 0) {
            this.setState(
                {
                    offset: this.state.offset-6,
                    limit: 6,
                },
                () => {
                    this.fetchData();
                }
            );
        }
    }

    handleGenderClick = () => {
        this.setState({openGenderDropdown : !this.state.openGenderDropdown});
    }

    setGenderClick = (option) => {
        this.setState({gender: option, openGenderDropdown: false},
            ()=>{
                this.fetchData();
            }
        );
    }

    handleLocationClick = () => {
        this.setState({openLocationDropdown : !this.state.openLocationDropdown,searchByLocation: ""});
    }

    handleLocationOutsideClick = () => {
        this.setState({openLocationDropdown : false});
    }


    setLocationClick = (option) => {
        this.setState({location: option, openLocationDropdown: false},
            ()=>{
                this.fetchData();
            }
        ); 
    }

    handleSearchPlaces = (input) => {

        console.log(input);
        if(this.state.searchByLocation !== "") {

        }
        input = input+"";

        input = input.trimStart();
        if(input?.length >= 3) {
            this.setState({searchByLocation: input, showLocationSuggestion: true});
            let locationSuggestions = this.state.totalData?.map((x)=>{
                console.log(x?.city?.toString().toLowerCase())
                let possibleCity = x?.address?.city?.toString()?.toLowerCase();
                let possibleState = x?.address?.state?.toString()?.toLowerCase();
                console.log(possibleCity);
                console.log(typeof(possibleCity));
                if(possibleCity.includes(input.toString().toLowerCase()) || possibleState.includes(input.toString().toLowerCase())) {
                    return (x.address.city+","+x.address.state);
                }
                return null;
            });
            locationSuggestions = locationSuggestions.filter((x,index) => (x!=null && locationSuggestions.indexOf(x) === index ))
            console.log(locationSuggestions);
            this.setState({locationSuggestionList: locationSuggestions});
        }
        else if(input?.length < 3){
            this.setState({searchByLocation: input,locationSuggestionList:[],showLocationSuggestion:false},
            );
        }
    }

    setLocationSuggestion = (option) => {
        let address = option.split(",");
        console.log(address);
        console.log(address[0]);
        console.log(address[1]);
        console.log(this.state.data);
        let selectedLocationData = this.state.totalData?.filter((x)=> 
            (x?.address?.city?.toString()?.toLowerCase() === address[0].toString()?.toLowerCase() && x?.address?.state?.toString()?.toLowerCase() === address[1].toString()?.toLowerCase())
            );
        console.log(selectedLocationData);
        this.setState({data: selectedLocationData, searchByLocation:option, showLocationSuggestion: false});
    }

    clickRoom = (id) => {
        console.log(this.props.history);
        this.props.history.push({
            pathname: `toastData/${id}`
        })
        console.log(this.props.history);
    }
    
    render() {
        let loader = <Loader isLoading = {this.state.loading}></Loader>;
        const genderOptions = ['Gender','Male','Female','Any'];
        let locationOptions = this.state.totalData?.map((x)=>{
            return (x?.address?.state)
        })
        locationOptions =  [...new Set(locationOptions)];
        locationOptions = ["Select location", ...locationOptions];
        console.log(this.state.totalData);
        
        return (
            <div className="gap">
                {loader}
                <div className="btn-group">
                    <button onClick={this.handlePrevBtn} disabled={this.state.offset===0}>prev</button>
                    <button onClick = {this.handleNextBtn} disabled={(this.state.offset+this.state.limit)>=this.state.totalCount}>next</button>
                    <div style={{alignSelf: 'center'}}>{this.state.totalCount > 0 ? this.state.offset+1 : 0 } to {this.state.offset+this.state.limit} of {this.state.totalCount}</div>
                    <div className="searchBylocation">
                    <input type="search" className="location-search" placeholder="Search places..." onChange={(e)=>{this.handleSearchPlaces(e.target.value)}} value={this.state.searchByLocation}></input>
                    {(this.state.showLocationSuggestion && this.state.locationSuggestionList.length > 0) && (
                        <div ref={this.locationSuggestionDropdownRef} className="dropdown-location-suggestion">
                            {
                                this.state.locationSuggestionList.map((option)=>
                                    (<div onClick={()=>{this.setLocationSuggestion(option)}}className="dropdown-item-location-suggestion">{option}</div>)
                                )
                            }
                        </div>
                    )
                    }
                    </div>
                    <div className="dropdown-location">
                    <button onClick={this.handleLocationClick} className="dropdown-button-location">{this.state.location || 'Select location'}</button>
                        {this.state.openLocationDropdown &&
                        ( 
                            <div ref={this.locationDropdownRef} className="dropdown-menu">
                                {locationOptions.map((option,index)=>
                                    (<div key= {index} onClick={()=>this.setLocationClick(option)} className="dropdown-item-location">{option}</div>)
                                )}
                            </div>
                        )}
                    </div>
                    <div className="dropdown">
                    <button onClick={this.handleGenderClick} className="dropdown-button">{this.state.gender || 'Gender'}</button>
                        {this.state.openGenderDropdown &&
                        ( 
                            <div ref={this.genderDropdownRef}className="dropdown-menu">
                                {genderOptions.map((option,index)=>
                                    (<div key= {index} onClick={()=>this.setGenderClick(option)} className="dropdown-item">{option}</div>)
                                )}
                            </div>
                        )}
                    </div>
                </div>
                
                
                <div className="parent-layout">
                    {this.state.data?.map((element)=> {
                        return(
                            <div className="main-layout" onClick={()=>{this.clickRoom(element?.id)}}>
                                <div className="first-section">
                                    <img src={userLogo} alt="user-image" width={80} height={80}></img>
                                    <div className="details-section">
                                        <span style={{color:'teal'}}>{element.name}</span>
                                        <div style={{fontSize:'.875rem'}}>{element.address?.area+","+element.address?.city+","+element.address?.state}</div>
                                        <div style={{color:'grey'}}>vacancy : {element.requirement?.vacancy}</div>
                                    </div> 
                                </div>
                                <div className="second-section">
                                    <div>
                                        <div>size</div>
                                        <div>{element.requirement?.size}</div>
                                    </div>
                                    <div>
                                        <div>Rent</div>
                                        <div>{element?.amount}</div>
                                    </div>
                                    <div>
                                        <div>Looking for</div>
                                        <div>{element?.requirement?.gender}</div>
                                    </div>
                                </div>
                            </div>
                        )
                    })}
                </div>
            </div>
        )
    }
}
export default withRouter(AllRooms);