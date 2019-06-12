import React from 'react';
import {connect} from 'react-redux';
import {Button} from 'react-bootstrap';
import {format} from '../../components/Helpers';
import {Table, Checkbox} from 'react-bootstrap';
import moment from 'moment';

const PresencesOrUnavailabilitiesList = props => (
    <Table striped bordered hover>
        <thead>
        <tr>
            <td>Start time</td>
            <td>End time</td>
            <td>Title</td>
            <td>Description</td>
            <td>Is unavailability</td>
            <td>Modify</td>
            <td>Remove</td>
        </tr>
        </thead>
        <tbody>
        {props.presencesOrUnavailabilities.map(p =>
            <tr key={p.id}>
                <td>{moment.utc(p.startTime).local().format(format)}</td>
                <td>{p.endTime == null ? '----' : moment.utc(p.endTime).local().format(format)}</td>
                <td>{p.title}</td>
                <td>{p.description}</td>
                <td><Checkbox checked={p.location===null} disabled /></td>
                <td>
                    <Button
                        variant='primary'
                        onClick={() => props.modifyPresenceOrUnavailability(p)}
                    >
                        Modify
                    </Button>
                </td>
                <td>
                    <Button
                        variant='primary'
                        onClick={() => props.removePresenceOrUnavailability(p.id)}
                    >
                        Remove
                    </Button>
                </td>
            </tr>
        )}
        </tbody>
    </Table>
);

export default PresencesOrUnavailabilitiesList;