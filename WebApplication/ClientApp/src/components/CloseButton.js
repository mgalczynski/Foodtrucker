import React, { Fragment }from 'react';
import { Link } from 'react-router-dom';

const RenderInner = () =>
    <Fragment>
        <span aria-hidden='true'>&times;</span>
        <span className='sr-only'>Close</span>
    </Fragment>;

export default (props) =>
    props.to ?
        <Link role='button' type='button' className='close' to={props.to}>
            <RenderInner />
        </Link>
        :
        <button className='close' onClick={props.onClick}>
            <RenderInner />
        </button>;