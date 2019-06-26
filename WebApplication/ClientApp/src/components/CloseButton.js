import React, {Fragment} from 'react';
import {Link} from 'react-router-dom';
import {useTranslation} from 'react-i18next';

const RenderInner = (props) => {
    const {t} = useTranslation();
    return <Fragment>
        <span aria-hidden='true'>&times;</span>
        <span className='sr-only'>{t('Close')}</span>
    </Fragment>;
};

export default (props) =>
    props.to ?
        <Link role='button' type='button' className='close' to={props.to}>
            <RenderInner/>
        </Link>
        :
        <button className='close' onClick={props.onClick}>
            <RenderInner/>
        </button>;