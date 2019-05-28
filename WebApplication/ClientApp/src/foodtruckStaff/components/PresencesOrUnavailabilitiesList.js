import React from 'react';
import {connect} from 'react-redux';
import {Button} from 'react-bootstrap';
import {format} from '../../components/Helpers';
import {Table} from 'react-bootstrap';

const PresencesOrUnavailabilitiesList = props => (
    <Table striped bordered hover>
        <thead>
        <tr>
            <td>Start time</td>
            <td>End time</td>
            <td>Title</td>
            <td>Description</td>
            <td>Modify</td>
            <td>Remove</td>
        </tr>
        </thead>
        <tbody>
        {props.presencesOrUnavailabilities.map(p =>
            <tr key={p.id}>
                <td>{p.startTime.format(format)}</td>
                <td>{p.endTime == null ? '----' : p.endTime.format(format)}</td>
                <td>{p.title}</td>
                <td>{p.description}</td>
                <td>
                    <Button
                        variant='primary'
                        onClick={() => props.modifyPresence(p)}
                    >
                        Modify
                    </Button>
                </td>
                <td>
                    <Button
                        variant='primary'
                        onClick={() => props.removePresence(p.id)}
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