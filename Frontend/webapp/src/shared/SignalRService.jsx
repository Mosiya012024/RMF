// import * as signalR from '@microsoft/signalr';
// import store from '../redux/store/store';
// import { setRefreshScreenData, setSignalRMessage } from '../redux/actions';

// class SignalRService {
//     constructor() {
//         this.connection = null;
//     }

//     initializeSignalR = async () => {
//         this.connection = new signalR.HubConnectionBuilder()
//             .withUrl("https://localhost:7253/notificationHub", {
//                 skipNegotiation: true,
//                 transport: signalR.HttpTransportType.WebSockets
//             })
//             .configureLogging(signalR.LogLevel.Information)
//             .withAutomaticReconnect()
//             .build();

//         this.connection.start()
//             .then(() => {
//                 console.log("SignalR connection established.");
//                 store.dispatch(setRefreshScreenData({ shouldRefreshAllRoomsPage: false }));
//             })
//             .catch((err) => {
//                 console.error("SignalR connection error:", err);
//             });

//         // Handle events
//         this.connection.on('ChangedRoomDetails', (data) => {
//             if (data?.length > 0) {
//                 store.dispatch(setRefreshScreenData({ shouldRefreshAllRoomsPage: true }));
//             }
//         });

//         this.connection.on('RecieveMessage', (userName, message, MessageTime) => {
//             store.dispatch(setSignalRMessage({
//                 userName, 
//                 message, 
//                 MessageTime
//             }));
//         });

//         this.connection.onreconnecting((error) => {
//             console.log(`SignalR reconnecting. Error: ${error}`);
//         });

//         this.connection.onreconnected((connectionId) => {
//             console.log(`SignalR reconnected. Connection ID: ${connectionId}`);
//         });

//         this.connection.onclose((error) => {
//             console.error("SignalR connection closed. Error:", error);
//         });
//     };

//     joinRoom = (userName, chatRoom) => {
//         return this.connection.invoke("JoinRoom", { userName, chatRoom });
//     };

//     sendMessages = (message) => {
//         this.connection.invoke("SendMessage", message);
//     };

//     leaveChat = () => {
//         if (this.connection) {
//             this.connection.stop();
//         }
//     };
// }

// // Create a singleton instance of SignalRService
// const signalRServiceInstance = new SignalRService();
// export default signalRServiceInstance;
