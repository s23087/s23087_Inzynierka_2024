"use client";

import PropTypes from "prop-types";
import { useState } from "react";
import PaginationFooter from "../pagination_footer";
import PaginationWindow from "@/components/windows/pagination_window";

/**
 * Create footer with pagination switch, page navigation and pagination window.
 * @component
 * @param {object} props
 * @param {Number} props.page_qty Number of pages
 * @param {Number} props.max_instance_on_page Number of maximum number of object available on page
 * @param {Number} props.current_page Number of current page
 * @return {JSX.Element} footer element
 */
function WholeFooter({ max_instance_on_page, page_qty, current_page }) {
  // useState for showing pagination window
  const [windowShow, setWindowShow] = useState(false);
  return (
    <footer className="fixed-bottom w-100">
      <PaginationFooter
        max_instance_on_page={max_instance_on_page}
        page_qty={page_qty}
        current_page={current_page}
        paginationAction={() => setWindowShow(true)}
      />
      <PaginationWindow
        windowShow={windowShow}
        onHideFunction={() => setWindowShow(false)}
      />
    </footer>
  );
}

WholeFooter.propTypes = {
  page_qty: PropTypes.number.isRequired,
  max_instance_on_page: PropTypes.number.isRequired,
  current_page: PropTypes.number.isRequired,
};

export default WholeFooter;
