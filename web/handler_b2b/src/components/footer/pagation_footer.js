import PropTypes from "prop-types";
import Image from "next/image";
import { Button, Container, Stack } from "react-bootstrap";
import left_last_page from "../../../public/icons/left_last_page.png";
import left_page from "../../../public/icons/left_page.png";
import right_last_page from "../../../public/icons/right_last_page.png";
import right_page from "../../../public/icons/right_page.png";

function PagationFooter({ page_qty, max_instance_on_page, current_page }) {
  const footerStyle = {
    height: "66px",
  };
  const instanceChangerStyle = {
    "min-width": "54px",
    "max-width": "114px",
    height: "47px",
    "border-radius": "10px",
  };
  const pagationStyle = {
    "min-width": "290px",
    height: "47px",
    "border-radius": "10px",
  };
  return (
    <Stack
      className="main-blue-bg px-xl-5"
      style={footerStyle}
      direction="horizontal"
    >
      <Container className="pe-0">
        <span
          className="d-flex align-items-center justify-content-center main-text sec-blue-bg"
          style={instanceChangerStyle}
        >
          <p className="mb-0">{max_instance_on_page}</p>
        </span>
      </Container>
      <Container className="d-flex justify-content-end">
        <span
          className="d-flex align-items-center justify-content-center main-text sec-blue-bg"
          style={pagationStyle}
        >
          <Stack direction="horizontal">
            <Button variant="as-link" className="pe-0">
              <Image src={left_last_page} alt="To first page" />
            </Button>
            <Button variant="as-link" className="ps-0">
              <Image src={left_page} alt="To previous page" />
            </Button>
            <p className="mb-0">
              {current_page} ... {page_qty}
            </p>
            <Button variant="as-link" className="pe-0">
              <Image src={right_page} alt="To next page" />
            </Button>
            <Button variant="as-link" className="ps-0">
              <Image src={right_last_page} alt="To last page" />
            </Button>
          </Stack>
        </span>
      </Container>
    </Stack>
  );
}

PagationFooter.PropTypes = {
  page_qty: PropTypes.number.isRequired,
  max_instance_on_page: PropTypes.number.isRequired,
  current_page: PropTypes.number.isRequired,
};

export default PagationFooter;
