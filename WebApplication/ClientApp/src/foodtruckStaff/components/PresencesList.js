import React from 'react';
import {connect} from 'react-redux';
import {format} from '../../components/Helpers';
import {Table} from 'react-bootstrap';

const PresencesList = props => (
    <Table striped bordered hover>
        <thead>
        <tr>
            <td>Start time</td>
            <td>End time</td>
            <td>Title</td>
            <td>Description</td>
        </tr>
        </thead>
        <tbody>
        {props.presences.map(p =>
            <tr key={p.id}>
                <td>{p.startTime.format(format)}</td>
                <td>{p.startTime == null ? '----' : p.startTime.format(format)}</td>
                <td>{p.title}</td>
                <td>{p.description}</td>
            </tr>
        )}
        </tbody>
    </Table>
);

export default PresencesList;