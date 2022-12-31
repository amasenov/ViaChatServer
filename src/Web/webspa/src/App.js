import { useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { isNull, isFullString } from './utility';
import Lobby from './components/Lobby';
import Chat from './components/Chat';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

const App = () => {
  const [connection, setConnection] = useState();
  const [messages, setMessages] = useState([]);
  const [error, setError] = useState(null);
  const [users, setUsers] = useState([]);

  const joinRoom = async (user, room) => {
    try {
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:44382/chat")
        .configureLogging(LogLevel.Information)
        .build();

      connection.on("ReceiveMessage", (user, message) => {
        if(!isNull(error)){
          setError(null);
        }
        setMessages(messages => [...messages, { user, message }]);
      });

      connection.on("ReceiveError", (message) => {
        if(isFullString(message)) {
          setError(message);
        } else {
          setError(null);
        }
      });

      connection.on("UsersInRoom", (users) => {
        setUsers(users);
      });

      connection.onclose(e => {
        setConnection();
        setMessages([]);
        setUsers([]);
      });

      await connection.start();
      await connection.invoke("JoinRoom", { user, room });
      setConnection(connection);
    } catch (e) {
      console.log(e);
    }
  }

  const sendMessage = async (message) => {
    try {
      await connection.invoke("SendMessage", message);
    } catch (e) {
      console.log(e);
    }
  }

  const addRoom = async (user, room) => {
    try {
      await connection.invoke("AddRoom", { user, room });
      await connection.stop();
    } catch (e) {
      console.log(e);
    }
  }

  const closeConnection = async () => {
      try {
      //await connection.invoke("LeaveRoom");
      await connection.stop();
    } catch (e) {
      console.log(e);
    }
  }

  return <div className='app'>
    <h2>Via chat client</h2>
    <hr className='line' />
    {(!connection || !isNull(error)) 
      ? <Lobby joinRoom={joinRoom} error={error} addRoom={addRoom} />
      : <Chat sendMessage={sendMessage} messages={messages} users={users} closeConnection={closeConnection} />}
  </div>
}

export default App;
