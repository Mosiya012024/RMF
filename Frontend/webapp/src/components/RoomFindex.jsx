import ErrorBoundary from "../shared/ErrorBoundary";
import AllRooms from "./AllRooms";
import AllRoomsCopy from "./AllRoomsCopy";
import NavBar from "./NavBar";

export function RoomFinder() {
    return(
        <>
            {/* <NavBar></NavBar> */}
            {/* <AllRooms></AllRooms> */}
            <ErrorBoundary>
                <AllRoomsCopy></AllRoomsCopy>
            </ErrorBoundary>
        </>
    )
}