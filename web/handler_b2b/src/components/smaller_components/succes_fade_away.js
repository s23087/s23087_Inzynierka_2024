"use client"

import PropTypes from 'prop-types'
import { Col } from "react-bootstrap";

function SuccesFadeAway({showSuccess, setShowSuccess}){
    const transition = {
        opacity: 0,
        transition: "all 250ms linear 1.5s",
    };
    const beforeTransition = {
        opacity: 100,
        transition: "all 200ms linear 0s",
    };
    return (
        <Col className="text-end">
            <p
            className="mb-0 mt-3 green-main-text"
            style={showSuccess ? beforeTransition : transition}
            onTransitionEnd={() => setShowSuccess(false)}
            >
            Success!
            </p>
        </Col>
    )
}

SuccesFadeAway.PropTypes = {
    showSuccess: PropTypes.bool.isRequired,
    setShowSuccess: PropTypes.func.isRequired
}

export default SuccesFadeAway