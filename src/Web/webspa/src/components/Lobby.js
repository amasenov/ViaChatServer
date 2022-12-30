import { useState } from 'react';
import { Form, Button } from 'react-bootstrap';
import { isNull } from './../utility';

const Lobby = ({ joinRoom, error, addRoom }) => {
    const [user, setUser] = useState();
    const [room, setRoom] = useState();

    return <Form className='lobby'
        onSubmit={e => {
            e.preventDefault();
            if(isNull(error)){
                joinRoom(user, room);
            } else {
                addRoom(user, room);
            }
        }} >
        <Form.Group>
            <Form.Control placeholder="name" onChange={e => setUser(e.target.value)} />
            <Form.Control placeholder="room" onChange={e => setRoom(e.target.value)} />
        </Form.Group>
        <Button variant="success" type="submit" disabled={!user || !room}>{isNull(error) ? "Join" : "Add room"}</Button>
        {!isNull(error) ? (
            <>
                <p>{error}</p>
            </>
        ) : null}
    </Form>
}

export default Lobby;