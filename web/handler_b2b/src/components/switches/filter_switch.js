import PropTypes from "prop-types";
import Image from "next/image";
import { Button } from "react-bootstrap";
import active_filter_icon from "../../../public/switches/filter_true.png";
import inactive_filter_icon from "../../../public/switches/filter_false.png";

function FilterIcon({ is_active }) {
  return (
    <Button className="p-0" variant="as-link">
      {is_active === "true" ? (
        <Image
          src={active_filter_icon}
          alt="Active filter icon"
          priority={true}
        />
      ) : (
        <Image
          src={inactive_filter_icon}
          alt="Inactive filter icon"
          priority={true}
        />
      )}
    </Button>
  );
}

FilterIcon.PropTypes = {
  is_active: PropTypes.bool.isRequired,
};

export default FilterIcon;
