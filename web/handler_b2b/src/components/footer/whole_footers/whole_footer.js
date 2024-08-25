"use client";

import { useState } from "react";
import PagationFooter from "../pagation_footer";
import PagationWindow from "@/components/windows/pagation_window";

function WholeFooter({ max_instance_on_page, page_qty, current_page }) {
  const [windowShow, setWindowShow] = useState(false);
  return (
    <footer className="fixed-bottom w-100">
      <PagationFooter
        max_instance_on_page={max_instance_on_page}
        page_qty={page_qty}
        current_page={current_page}
        pagationAction={() => setWindowShow(true)}
      />
      <PagationWindow
        windowShow={windowShow}
        onHideFunction={() => setWindowShow(false)}
      />
    </footer>
  );
}

export default WholeFooter;
