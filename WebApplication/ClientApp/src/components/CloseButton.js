import React from 'react';
import { Link } from 'react-router-dom';

export default (props) =>
    <Link role='button' type='button' className='close' to={props.to}>
        <span aria-hidden='true'>&times;</span>
        <span className='sr-only'>Close</span>
    </Link>;