import PropTypes from "prop-types";
import Image from "next/image";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { Button, Container, Stack } from "react-bootstrap";
import left_last_page from "../../../public/icons/left_last_page.png";
import left_page from "../../../public/icons/left_page.png";
import right_last_page from "../../../public/icons/right_last_page.png";
import right_page from "../../../public/icons/right_page.png";

function PagationFooter({
  page_qty,
  max_instance_on_page,
  current_page,
  pagationAction,
}) {
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  const footerStyle = {
    height: "66px",
  };
  const instanceChangerStyle = {
    minWidth: "54px",
    maxWidth: "114px",
    height: "47px",
    borderRadius: "10px",
  };
  const pagationStyle = {
    minWidth: "290px",
    height: "47px",
    borderRadius: "10px",
  };
  return (
    <Stack
      className="main-blue-bg px-xl-5"
      style={footerStyle}
      direction="horizontal"
    >
      <Container className="pe-0">
        <span
          className="d-flex align-items-center justify-content-center sec-blue-bg"
          style={instanceChangerStyle}
        >
          <Button
            variant="white-border"
            className="p-0 w-100"
            onClick={pagationAction}
          >
            <p className="mb-0 main-text">{max_instance_on_page}</p>
          </Button>
        </span>
      </Container>
      <Container className="d-flex justify-content-end">
        <span
          className="d-flex align-items-center justify-content-center main-text sec-blue-bg"
          style={pagationStyle}
        >
          <Stack direction="horizontal">
            <Button
              variant="white-border"
              className="p-0"
              onClick={(e) => {
                e.preventDefault();
                setPage(1);
              }}
            >
              <Image src={left_last_page} alt="To first page" />
            </Button>
            <Button
              variant="white-border"
              className="p-0"
              onClick={(e) => {
                e.preventDefault();
                if (current_page - 1 <= 0) return;
                setPage(current_page - 1);
              }}
            >
              <Image src={left_page} alt="To previous page" />
            </Button>
            <p className="mb-0 px-2">
              {current_page} ... {page_qty}
            </p>
            <Button
              variant="white-border"
              className="p-0"
              onClick={(e) => {
                e.preventDefault();
                if (current_page + 1 > page_qty) return;
                setPage(current_page + 1);
              }}
            >
              <Image src={right_page} alt="To next page" />
            </Button>
            <Button
              variant="white-border"
              className="p-0"
              onClick={(e) => {
                e.preventDefault();
                setPage(page_qty);
              }}
            >
              <Image src={right_last_page} alt="To last page" />
            </Button>
          </Stack>
        </span>
      </Container>
    </Stack>
  );

  function setPage(page_number) {
    const newParams = new URLSearchParams(params);
    newParams.set("page", page_number);
    router.replace(`${pathName}?${newParams}`);
  }
}

PagationFooter.propTypes = {
  page_qty: PropTypes.number.isRequired,
  max_instance_on_page: PropTypes.number.isRequired,
  current_page: PropTypes.number.isRequired,
  pagationAction: PropTypes.func.isRequired,
};

export default PagationFooter;
