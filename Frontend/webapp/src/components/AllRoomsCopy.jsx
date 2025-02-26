import { Component, createRef } from "react";
import React from "react";
import axios from "axios";
import './AllRooms.css';
import userLogo from "../images/user-logo.png"
import Loader from "../shared/Loader";
import { withRouter } from "react-router-dom";
import { IconButton, Switch } from "@mui/material";
import DCCGridv1 from "../shared/DCCGridv1";
import DCCGridv2 from "../shared/DCCGridv2";
import FirstPageIcon from '@mui/icons-material/FirstPage';
import LastPageIcon from '@mui/icons-material/LastPage';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import PostRoomDetails from "./PostRoomDetails";
import dayjs from "dayjs";
import { toast } from "react-toastify";
import EditIcon from '@mui/icons-material/Edit';
import DeleteForeverIcon from '@mui/icons-material/DeleteForever';
import { connect } from "react-redux";
import { setSelectedUserName } from "../redux/actions/SelectedUserName";
import { setSelectedUserType } from "../redux/actions/SelectedUserType";
import { rootReducer } from "../redux/store/store";
import { bindActionCreators } from "redux";
import store from "../redux/store/store";
import NoListingFoundImage from '../images/data-not-found.svg'
import KeyboardArrowLeft from '@mui/icons-material/KeyboardArrowLeft';
import KeyboardArrowRight from '@mui/icons-material/KeyboardArrowRight';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import Box from '@mui/material/Box';
import BedroomChildOutlinedIcon from '@mui/icons-material/BedroomChildOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import FormatListBulletedOutlinedIcon from '@mui/icons-material/FormatListBulletedOutlined';
import axiosInstance from "../shared/axiosInstanceInterceptor";
import ErrorBoundary from "../shared/ErrorBoundary";
import { setRefreshScreenData } from "../redux/actions/RefreshScreenData";
import { InitializeSignalR } from "../shared/NotificationService";
import ChatIcon from '@mui/icons-material/Chat';
import signalRServiceInstance from "../shared/NotificationService2";
import { setCurrentUserInfo } from "../redux/actions/CurrentUserInfo";
import * as Utility from '../shared/Utility';
import CheckCircleOutlinedIcon from '@mui/icons-material/CheckCircleOutlined';


class AllRoomsCopy extends Component {
    constructor(props) {
        super(props);
        let urlParams = new URLSearchParams(this.props.location.search);
        this.state = {
            data : [],
            loading: false,
            totalCount: 0,
            offset: 0,
            limit: 6,
            openGenderDropdown: false,
            gender:  urlParams.get('gender') !== null ? urlParams.get('gender') : "Gender",
            location: urlParams.get('location') !== null ? urlParams.get('location') : "Select location",
            openLocationDropdown: false,
            totalData: [],
            showLocationSuggestion: false,
            searchByLocation : "",
            locationSuggestionList: [],
            isSwitchTableView: false,
            orderBy:"",
            openEditRoomPopup:false,
            roomCombinedDetails:{},
            tabValue: "",
        }
        this.locationDropdownRef = createRef();
        this.genderDropdownRef = createRef();
        this.locationSuggestionDropdownRef = createRef();
    }

     

    fetchData() {
        let token = localStorage.getItem('userToken');
        let urlParams = new URLSearchParams(this.props.location.search);
        console.log(urlParams);
        let genderFilters = urlParams.get('gender');
        console.log(genderFilters);
        let nameFilters = urlParams.get('location');
        console.log(nameFilters);
        let genderFilter = (urlParams.get('gender') !== null) ? `gender eq ${urlParams.get('gender')}` : "";
        let locationFilter = (urlParams.get('location') !== null) ? `location eq ${urlParams.get('location')}` : "";
        let userTypeFilter = (urlParams.get('userType') != null) ? `userType eq ${urlParams.get('userType')}`:"";
        let data;
        if(genderFilter !== "" && locationFilter !== "" && userTypeFilter !== "") {
            data = genderFilter + " (and) " + locationFilter + " (and) " + userTypeFilter;
        }
        else if(genderFilter !== "" && locationFilter !== "") {
            data = genderFilter + " (and) " + locationFilter;
        }
        else if(genderFilter !== "" && userTypeFilter !== "") {
            data = genderFilter + " (and) " + userTypeFilter;
        }
        else if(locationFilter !== "" && userTypeFilter !=="") {
            data = locationFilter + " (and) " + userTypeFilter;
        }
        else {
            data = genderFilter+locationFilter+userTypeFilter;
        }
        
        console.log(token);
        
        //axios.get(`https://localhost:44356/get/allRooms?offset=${this.state.offset}&limit=${this.state.limit}&orderBy=${this.state.orderBy}&filter=`+data ,
        axios.get(`https://localhost:7253/get/allRooms?offset=${this.state.offset}&limit=${this.state.limit}&orderBy=${this.state.orderBy}&filter=`+data ,
            {
            headers: {
              "Authorization": `Bearer ${token}`
            }
          })
        .then(
            (response) => {
                console.log(response?.data);
                console.log(response?.data?.data);
                this.setState({data:response?.data?.data, loading: false, totalCount: response?.data?.count,tabValue: `${urlParams.get('userType')}` === 'null' ? "" : `${urlParams.get('userType')}`});
            }
        )
        .catch(
            (error) => {
                console.log(error);
                //throw new Error('I crashed');
                
            }
        )
    }

    //componentDidUpdate(prevState) {
        // console.log(prevState.gender);
        // console.log(this.state.gender);
        // if(this.state.gender !== prevState.gender) {
        //     this.fetchData();
        // }
        //this.fetchData();
        
    //}

    componentDidUpdate() {
        console.log(this.props.selectedrefreshScreenData);
        if(this.props.selectedrefreshScreenData.RefreshScreenData.shouldRefreshAllRoomsPage === true) {
            let modifiedRefreshScreenData = {...this.props.selectedrefreshScreenData.RefreshScreenData, shouldRefreshAllRoomsPage: false};
            // this.props.selectedrefreshScreenData.RefreshScreenData.shouldRefreshAllRoomsPage = false;
            this.props.setRefreshScreenData(modifiedRefreshScreenData);
            console.log(this.props.selectedrefreshScreenData);
            this.fetchData();
        }
    }


    fetchTotalData = async () => {
        const token = localStorage.getItem('userToken');
        try {
            const response = 
            //await axios.get(`https://localhost:44356/get/allRooms?offset=0&limit=1000&orderBy=&filter=`, {
            await axios.get(`https://localhost:7253/get/allRooms?offset=0&limit=1000&orderBy=&filter=`, {
            headers: {
              "Authorization": `Bearer ${token}`
            }
          });
          this.setState({ totalData: response?.data?.data });
        }
        catch (error) {
          // Instead of throwing here, update the state to trigger a re-render with an error
          throw new Error('I crashed');
        }
      }

      fetchRooms = () => {
        axiosInstance.get('/get/allRooms', {
          params: {
            offset: 0,
            limit: 1000,
            orderBy: '',
            filter: ''
          }
        })
        .then(response => {
          this.setState({ totalData: response?.data?.data });
        })
        .catch(error => {
          console.log(error);
        });
      }
    

    componentDidMount() {
        let token = localStorage.getItem('userToken');
        this.fetchData();
        //this.fetchTotalData();
        this.fetchRooms();
        // axiosInstance.get('/get/allRooms', {
        //     params: {
        //       offset: 0,
        //       limit: 1000,
        //       orderBy: '',
        //       filter: ''
        //     }
        //   })
        //   .then(response => {
        //     this.setState({ totalData: response?.data?.data });
        //   })
        //   .catch(error => {
        //     console.error(error);
        //     throw new Error('I crashed'); // This will be caught by the Error Boundary
        //   });
        // axios.get(`https://localhost:44356/get/allRooms?offset=0&limit=1000&orderBy=&filter=`,{
        //     headers: {
        //       "Authorization": `Bearer ${token}`
        //     }
        //   })
        // .then(
        //     (response) => {
        //         this.setState({totalData:response?.data?.data});
        //     }
        // )
        // .catch(
        //     (error) => {
        //         console.log(error);
        //         throw new Error('I crashed');
                
        //     }
        // )
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

    updateSearchParams = (key,value) => {
        let searchParams = new URLSearchParams(this.props.location.search);
        if((key === "gender" && value === "Gender") || (key === "location" && value === "Select location") || (key === "userType" && value === "")) {
            searchParams.delete(key);
        }

        else {
            searchParams.set(key,value);
        }

        this.props.history.push({
            pathname: this.props.location.pathname,
            search: searchParams.toString(),
        });
    }

    handleGenderClick = () => {
        this.setState({openGenderDropdown : !this.state.openGenderDropdown});
    }

    setGenderClick = (option) => {
        // this.props.history.push(
        //     {
        //         pathname: this.props.history.pathname+`?gender=${option}&name=Mosiya`,
        //     });
        //this.props.setSelectedUserName("Mosiya Syeda");
        this.updateSearchParams('gender',option)
        this.setState({gender:option, openGenderDropdown: false},()=>this.fetchData())
    }

    handleLocationClick = () => {
        this.setState({openLocationDropdown : !this.state.openLocationDropdown,searchByLocation: ""});
    }

    handleLocationOutsideClick = () => {
        this.setState({openLocationDropdown : false});
    }


    setLocationClick = (option) => {
        this.props.setSelectedUserType("Admin");
        this.updateSearchParams('location',option)
        this.setState({location: option, openLocationDropdown: false},()=>this.fetchData()); 
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

    clickRoom = (userIdentity, id, clickedName) => {
        console.log(this.props.history);
        localStorage.setItem('clickedListing',clickedName);
        this.props.history.push({
            pathname: `${encodeURIComponent(userIdentity)}/${id}`
        })
        console.log(this.props.history);
    }

    handleChangeSwitch = () => {
        this.setState({isSwitchTableView: !this.state.isSwitchTableView})
    }

    getAllKeys = (templateData,allKeys) => {
        Object.keys(templateData).map((x)=>{
            if(typeof templateData[x] === 'object' && x !== "address") {
                return this.getAllKeys(templateData[x],allKeys);
            }
            else {
                return allKeys.push(x);
            }
        })
    }

    formatDateForTableView = (dataToFormat) => {
        let formattedData = dataToFormat.map((x)=>{

            //creating a shallow copy of every object in the array
            let newObj = JSON.parse(JSON.stringify(x));
            newObj["area"] = newObj.address?.area;
            newObj["city"] = newObj.address?.city;
            newObj["state"] = newObj.address?.state;
            let totalAddress = newObj.address?.area+","+newObj.address?.city+","+newObj.address?.state;
            delete newObj["address"];
            newObj["address"] = totalAddress;
            newObj.size = newObj.requirement.size;
            newObj.vacancy = newObj.requirement.vacancy;
            newObj.gender = newObj.requirement.gender;
            newObj["more"] = <MoreVertIcon></MoreVertIcon>;
            
            delete newObj["requirement"];
            return newObj;
        })
        console.log(formattedData);
        return formattedData;
    }

    setOrderBy = (orderByString) => {
        this.setState({orderBy: orderByString},()=>this.fetchData());
    }

    navigateToFirstPage = () => {
        this.setState({offset:0, limit:this.state.totalCount>6 ? 6: this.state.totalCount},()=>this.fetchData())
    }

    navigateToLastPage = () => {
        if(this.state.totalCount<=6) {
            this.setState({offset:0,limit:this.state.totalCount},()=>{this.fetchData()});
            return;
        }
        let quotient = this.state.totalCount/6;
        let rem = this.state.totalCount%6;
        if(rem === 0) {
            this.setState({offset: this.state.totalCount-6,limit:6},()=>{this.fetchData()});
        }
        else {
            this.setState({offset: this.state.totalCount-rem,limit: rem},()=>{this.fetchData()});
        }
    }

    buildEditedData = (RoomId, roomDetails,RoomDescription) => {
        console.log(roomDetails);
        console.log(RoomDescription)
        var userEditedData = {
            "id":RoomId,
            "name":roomDetails.name,
            "roomType":roomDetails.identity === "Room Mate Finder" ? "Room" : "Room mate",
            "area": roomDetails.area,
            "city":roomDetails.city,
            "state":roomDetails.state,
            "size":roomDetails.size,
            "vacancy":roomDetails.vacancy,
            "gender":roomDetails.gender,
            "message":roomDetails.message,
            "rent":roomDetails.amount,
            "deposit":RoomDescription.deposit,
            "maintenance":RoomDescription.maintenance,
            "amenityType": RoomDescription.amenityType,
            "amenitiesList": RoomDescription.amenities?.length === 0 ? [] : [...RoomDescription.amenities],
            "status": roomDetails.status,
            "flatNumber": RoomDescription.flatNumber ?? "",
            "availableFrom": dayjs(RoomDescription.availableFrom),
            "postedOn": RoomDescription.postedOn,
            "identity": roomDetails.identity,
        }
        console.log(userEditedData);
        this.setState({roomCombinedDetails: userEditedData});
        
    }

    fetchRoomDescription = (RoomId,roomDetails) => {
        //axios.get(`https://localhost:44356/getRDX?filterId=${RoomId}`,{
        axios.get(`https://localhost:7253/getRDX?filterId=${RoomId}`,{
            headers:{
                'Authorization': `Bearer ${localStorage.getItem('userToken')}`
            }
        })
        .then((res)=>{
            console.log(res);
            
            this.setState({RoomDescription: res?.data?.data,openEditRoomPopup: true},()=>this.buildEditedData(RoomId,roomDetails,this.state.RoomDescription));
        })
        .catch((error)=>{
            console.log(error);
        })
    }

    closeEditRoomPopup = () => {
        this.setState({openEditRoomPopup:false});
    }

    deleteRoom = (deleteRoomId,deleteUserIdentity) => {
        //axios.delete(`https://localhost:44356/delete/roomCombinedDetails?filterId=${deleteRoomId}`,
        axios.delete(`https://localhost:7253/delete/roomCombinedDetails?filterId=${deleteRoomId}`,
            {
                headers : {
                    'Authorization' : `Bearer ${localStorage.getItem('userToken')}`
                }
            }
        )
        .then((res)=>{
            console.log(res);
            toast.success(`${deleteUserIdentity}`+ " deleted Sucessfully !",{
                position: 'top-center',
                autoClose: 5000
            });
        })
        .catch((error)=>{
            console.log(error);
        })
    }

    editRoomfromNormalView = (normalViewData) => {
        let newObj = JSON.parse(JSON.stringify(normalViewData));
        newObj["area"] = newObj.address?.area;
        newObj["city"] = newObj.address?.city;
        newObj["state"] = newObj.address?.state;
        let totalAddress = newObj.address?.area+","+newObj.address?.city+","+newObj.address?.state;
        delete newObj["address"];
        newObj["address"] = totalAddress;
        newObj.size = newObj.requirement.size;
        newObj.vacancy = newObj.requirement.vacancy;
        newObj.gender = newObj.requirement.gender;
            
        delete newObj["requirement"];
        this.fetchRoomDescription(normalViewData["id"],newObj);
            
    }

    NoListingFound = () => {
        let NoListingFoundData = <></>;
        if(this.state.data?.length === 0) {
            NoListingFoundData = (
                <div className="no-listing-found">
                    <img src={NoListingFoundImage} width="224" height="224"/>
                    <p>No Listing Found</p>
                    <div>We couldn't find any listing that matches your search.</div>
                    <div>You can try to search nearby places.</div>
                </div>
            )
        }
        return NoListingFoundData;
    }

    handleChangeTab = (event,newValue) => {
        //This code can be used only when you want to filter the data based on totalData, without making an api call
        // let filterIdenity = newValue === "Rooms" ? "Room Mate Finder" : newValue === "Room mates" ? "Room Finder": "";
        // let filteredDataBasedonTab
        // if(filterIdenity !== "") {
        //     filteredDataBasedonTab = this.state.totalData.filter((x)=>x.identity === filterIdenity);
        // }
        // else {
        //     filteredDataBasedonTab = this.state.totalData.filter(() => true);
        // }
        
        // this.state.totalData.filter((x)=>x.identity === filterIdenity);
        // this.setState({tabValue: newValue,data: filteredDataBasedonTab})

        //This code is used to make an api call on change of every tab in the UI, we will send that userType in filter component.
        //let filterUserIdentity = newValue === "Rooms" ? "Room Mate Finder" : newValue === "Room mates" ? "Room Finder": "";
        this.updateSearchParams('userType',newValue);
        this.setState({tabValue: newValue},()=>this.fetchData());
    }

    

    handleChatWithUser = async (userIdentity,userName) => {
        let generatedRoom = Utility.generateRoomName(userName,this.props.currentUserInfo.CurrentUserInfo?.name);
        this.props.setSelectedUserName(generatedRoom);
        console.log(generatedRoom);
        
        this.props.history.push({
            //pathname: `${encodeURIComponent(userIdentity)}/chat/${userName}`
            pathname: `/chat-with/user/${userName}`
        });

        console.log(this.props.selectedUserName);
        if (signalRServiceInstance && signalRServiceInstance.connection && signalRServiceInstance.connection.state === 'Connected') {
            let abc = store.getState()
            let answer = await signalRServiceInstance.checkFirstJoined(this.props.currentUserInfo.CurrentUserInfo?.name, generatedRoom);
            
            if(answer) {
                console.log("yes happeinig");
                await signalRServiceInstance.sendChatRequest(userName,this.props.currentUserInfo.CurrentUserInfo?.name);
            }
            signalRServiceInstance.joinRoom(this.props.currentUserInfo.CurrentUserInfo?.name, generatedRoom);
            
        } else {
            console.error('SignalR connection is not initialized or ready yet.');
            // Optionally, you can retry or notify the user
        }
        //signalRServiceInstance.joinRoom(userName,generatedRoom);
    }

    handleBookRequest = (toUser_Name, toBeMatchedID) => {
        signalRServiceInstance.sendBookRequest(toUser_Name,this.props.currentUserInfo.CurrentUserInfo?.name,toBeMatchedID);
    }
    
    render() {
        console.log(this.props.selectedrefreshScreenData);
        console.log(this.state.tabValue)
        console.log(this.state.data);
        console.log(store.getState())
        console.log(this.props.selectedUserName.SelectedUserName);
        console.log(this.props.selectedUserType);
        console.log(localStorage.getItem('userToken'));
        let templateData = {
            id: "66a2aab47059f43c536c6155",
            name: "Olivia",
            address: {
                area: "Greater Kailash",
                city: "NCR",
                state: "Delhi"
            },
            requirement: {
                size: "1BHK",
                vacancy: 8,
                gender: "Female",
                message: "nkjfjd"
            },
            amount: 9000,
            status: "Vacant",
            identity: "Room Mate Finder",
        }
        let allKeys = [];
        this.getAllKeys(templateData,allKeys);
        allKeys.splice(0,1);
        let statusIndex = allKeys.indexOf("message");
        allKeys.splice(statusIndex,1);
        allKeys.push("more")
        console.log(allKeys);
        console.log(this.state.isSwitchTableView);
        let loader = <Loader isLoading = {this.state.loading}></Loader>;
        const genderOptions = ['Gender','Male','Female','Any'];
        let locationOptions = this.state.totalData?.map((x)=>{
            return (x?.address?.state)
        })
        locationOptions =  [...new Set(locationOptions)];
        locationOptions = ["Select location", ...locationOptions];
        console.log(this.state.totalData);
        console.log(this.state.roomCombinedDetails);
        return (
            
            <div className="gap">
                {loader}
                <PostRoomDetails editingData={true} open={this.state.openEditRoomPopup} handleClose={this.closeEditRoomPopup} roomCombinedDetails={this.state.roomCombinedDetails}>
                </PostRoomDetails>
                <div className="btn-group">
                    {/* <IconButton disabled={this.state.offset===0} onClick={this.navigateToFirstPage}>
                        <FirstPageIcon></FirstPageIcon>
                    </IconButton>
                    <button onClick={this.handlePrevBtn} disabled={this.state.offset===0}>prev</button>
                    <button onClick = {this.handleNextBtn} disabled={(this.state.offset+this.state.limit)>=this.state.totalCount}>next</button>
                    <IconButton disabled={(this.state.offset+this.state.limit)>=this.state.totalCount} onClick={this.navigateToLastPage}>
                        <LastPageIcon></LastPageIcon>
                    </IconButton>
                    <div style={{alignSelf: 'center'}}>{this.state.totalCount > 0 ? this.state.offset+1 : 0 } to {this.state.totalCount>0 ? this.state.offset+this.state.limit: 0} of {this.state.totalCount}</div>
                    <Switch
                        checked={this.state.isSwitchTableView}
                        onChange={()=>this.handleChangeSwitch()}
                        inputProps={{ 'aria-label': 'controlled' }}
                    /><span style={{alignSelf:'center'}}>Table View</span> */}
                    <Box sx={{ width: '100%' }}>
                        <Tabs
                            value={this.state.tabValue}
                            onChange={this.handleChangeTab}
                            textColor="secondary"
                            indicatorColor="secondary"
                            aria-label="secondary tabs example"
                        >
                            <Tab value="" label="All Listing" icon={<FormatListBulletedOutlinedIcon/>} style={{textTransform: 'capitalize'}}></Tab>
                            <Tab value="Room Mate Finder" label="Rooms" icon={<BedroomChildOutlinedIcon/>} style={{textTransform: 'capitalize'}}/>
                            <Tab value="Room Finder" label="Room mates" icon={<GroupsOutlinedIcon/>}style={{textTransform: 'capitalize'}}/>
                        </Tabs>
                    </Box>
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
                <br></br>
                <div className="horizontal-line"></div>
                
                {this.state.isSwitchTableView ? 
                    (<div style={{margin:'15px 35px 15px 35px'}}>
                        {/* <DCCGridv1 
                            columns={allKeys}
                            rowsData={this.formatDateForTableView(this.state.data)}
                        >
                        </DCCGridv1> */}
                        <DCCGridv2 
                            columns={allKeys}
                            rowsData={this.formatDateForTableView(this.state.data)}
                            setOrderByString={this.setOrderBy}
                            openEditRoomPopup={this.fetchRoomDescription}
                            deleteRoom={this.deleteRoom}
                            clickRoom={this.clickRoom}
                        >
                        </DCCGridv2>
                        <br></br>
                    </div>) : 
                (<div className="parent-layout">
                    {this.NoListingFound()}
                    {this.state.data?.map((element)=> {
                        return(
                            <div className="main-layout">
                                <div className="first-section">
                                    <img src={userLogo} alt="user-image" width={80} height={80}></img>
                                    <div className="details-section">
                                        <div className="name-icon-section">
                                            <div style={{color:'teal'}}>{element.name}</div>
                                            <div>
                                                <IconButton onClick={()=>this.editRoomfromNormalView(element)}>
                                                    <EditIcon></EditIcon>
                                                </IconButton>
                                                <IconButton onClick={()=>this.deleteRoom(element?.id,element?.identity)}>
                                                    <DeleteForeverIcon></DeleteForeverIcon>
                                                </IconButton>
                                            </div>
                                        </div>
                                        <div className="chat-icon">
                                            <div>
                                                <div className="area-name" style={{fontSize:'.875rem'}}>{element.address?.area+","+element.address?.city+","+element.address?.state}</div>
                                                <div style={{color:'grey'}}>
                                                    {
                                                        element?.identity === "Room Mate Finder" ? 
                                                        `vacancy: ${element.requirement?.vacancy}` : `Occupancy: ${element.requirement.size}`
                                                    }
                                                    
                                                </div>
                                            </div>
                                            <div className="chat-match-icons">
                                                <div>
                                                    <IconButton style={{color:'#0073CF'}} onClick={()=>{this.handleChatWithUser(element.identity,element.name)}}>
                                                        <ChatIcon></ChatIcon>
                                                    </IconButton>
                                                </div>
                                                <div>
                                                    {   
                                                        ((element.identity === "Room Mate Finder" && this.props.currentUserInfo.CurrentUserInfo?.usertype === "Room Finder") ||
                                                        (element.identity === "Room Finder" && this.props.currentUserInfo.CurrentUserInfo?.usertype === "Room Mate Finder")) &&
                                                        (<IconButton style={{color:'#0073CF'}} onClick={()=>{this.handleBookRequest(element?.name,element?.id)}}>
                                                            <CheckCircleOutlinedIcon></CheckCircleOutlinedIcon>
                                                        </IconButton>)
                                                    }
                                                </div>
                                            </div>
                                        </div>
                                    </div> 
                                </div>
                                <div className="second-section" onClick={()=>{this.clickRoom(element.identity,element?.id,element.name)}}>
                                    <div className={`${element?.identity}` === "Room Finder" ? "room-finder" : ""}>
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
                                    <div>
                                        <div>I am a</div>
                                        <div style={{fontStyle:'italic',fontSize: '11px',}}>{element?.identity}</div>
                                    </div>
                                </div>
                            </div>
                        )
                    })}
                </div>)}
                <div className="btn-group-pagination">
                    <IconButton disabled={this.state.offset===0} onClick={this.navigateToFirstPage}>
                        <FirstPageIcon></FirstPageIcon>
                    </IconButton>
                    <IconButton onClick={this.handlePrevBtn} disabled={this.state.offset===0}>
                        <KeyboardArrowLeft></KeyboardArrowLeft>
                    </IconButton>
                    <IconButton onClick = {this.handleNextBtn} disabled={(this.state.offset+this.state.limit)>=this.state.totalCount}>
                        <KeyboardArrowRight></KeyboardArrowRight>
                    </IconButton>
{/* 
                    <button onClick={this.handlePrevBtn} disabled={this.state.offset===0}>prev</button>
                    <button onClick = {this.handleNextBtn} disabled={(this.state.offset+this.state.limit)>=this.state.totalCount}>next</button> */}
                    <IconButton disabled={(this.state.offset+this.state.limit)>=this.state.totalCount} onClick={this.navigateToLastPage}>
                        <LastPageIcon></LastPageIcon>
                    </IconButton>
                    <div style={{alignSelf: 'center'}}>{this.state.totalCount > 0 ? this.state.offset+1 : 0 } to {this.state.totalCount>0 ? this.state.offset+this.state.limit: 0} of {this.state.totalCount}</div>
                    <Switch
                        checked={this.state.isSwitchTableView}
                        onChange={()=>this.handleChangeSwitch()}
                        inputProps={{ 'aria-label': 'controlled' }}
                    /><span style={{alignSelf:'center'}}>Table View</span>
                </div>
            </div>
            
        )
    }
}

const mapStateToProps = (state=rootReducer) => ({
    selectedUserName: state.SelectedUserName,
    selectedUserType: state.SelectedUserType,
    selectedrefreshScreenData: state.RefreshScreenData,
    currentUserInfo: state.CurrentUserInfo,
    signalRObject: state.SignalRMessage,
})
const mapDispatchToProps = (dispatch) => ({
    setSelectedUserName: bindActionCreators(setSelectedUserName, dispatch),
    setSelectedUserType: bindActionCreators(setSelectedUserType, dispatch),
    setRefreshScreenData: bindActionCreators(setRefreshScreenData, dispatch),
    setCurrentUserInfo: bindActionCreators(setCurrentUserInfo,dispatch)
});



// const mainAllRoomsCopy = connect(mapStateToProps,mapDispatchToProps)(withRouter(AllRoomsCopy));
// //const AllRoomsCopyObject = new withRouter(AllRoomsCopy);
// const EnhancedAllRoomsCopy = withRouter(connect(mapStateToProps, mapDispatchToProps)(AllRoomsCopy));
// const abc = new EnhancedAllRoomsCopy();

// export  default { abc, mainAllRoomsCopy}
//export default withRouter(AllRoomsCopy);
export default connect(mapStateToProps,mapDispatchToProps)(withRouter(AllRoomsCopy));