//Normal redux store without persist store and persist reducer

// import { configureStore } from '@reduxjs/toolkit'
// import { SelectedUserNameReducer } from '../reducers/SelectedUserNameReducer';
// import { combineReducers } from '@reduxjs/toolkit';


// export const rootReducer = combineReducers({
//     SelectedUserName: SelectedUserNameReducer
// })

// const store = configureStore({
//   reducer: rootReducer,
//   });
// export default store;


import { configureStore } from '@reduxjs/toolkit'
import { SelectedUserNameReducer } from '../reducers/SelectedUserNameReducer';
import { SelectedUserTypeReducer } from '../reducers/SelectedUserTypeReducer';
import { CurrentUserInfoReducer } from '../reducers/CurrentUserInfoReducer';
import { combineReducers } from '@reduxjs/toolkit';
import { persistStore } from 'redux-persist';
import { persistReducer } from 'redux-persist';
import storage from 'redux-persist/lib/storage';
import { APIResponseCodeReducer } from '../reducers/APIResponseCodeReducer';
import { RefreshScreenDataReducer } from '../reducers/RefreshScreenDataReducer';
import { SignalRMessageReducer } from '../reducers/SignalRMessageReducer';


const persistConfig = {
    key: 'root',    
    storage,        
    whitelist: ['SelectedUserName','SelectedUserType','CurrentUserInfo'],  
  };

export const rootReducer = combineReducers({
    SelectedUserName: SelectedUserNameReducer,
    SelectedUserType: SelectedUserTypeReducer,
    CurrentUserInfo: CurrentUserInfoReducer,
    APIResponseCode: APIResponseCodeReducer,
    RefreshScreenData: RefreshScreenDataReducer,
    SignalRMessage: SignalRMessageReducer,
})

const persistedReducer = persistReducer(persistConfig, rootReducer);

const store = configureStore({
  reducer: persistedReducer,
  });

export const persistor = persistStore(store);
export default store;
