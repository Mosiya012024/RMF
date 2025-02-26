import { combineReducers, configureStore } from "@reduxjs/toolkit";
import { CheckReducer } from "../reducers/CheckReducer";
import storage from 'redux-persist/lib/storage';
import persistReducer from "redux-persist/es/persistReducer";
import persistStore from "redux-persist/es/persistStore";

// export const AllReducers = combineReducers({
//     CheckAction: CheckReducer
// })
// const store2 = configureStore({
//     reducer: AllReducers
// })

// export default store2;

const persistConfigs= {
    key: 'root',    
    storage,    
    whitelist: ['SelectedUserName','SelectedUserType','CurrentUserInfo'],  
  };

export const AllReducers = combineReducers({
    CheckAction: CheckReducer,
})

const persistedReducers = persistReducer(persistConfigs, AllReducers);

const store2 = configureStore({
  reducer: persistedReducers,
  });

export const persistor2 = persistStore(store2);
export default store2;
