import PropTypes from "prop-types";
import { Stack, Container } from "react-bootstrap";
import Image from "next/image";
import SearchBar from "../smaller_components/search_bar";
import FilterIcon from "../switches/filter_switch";
import icon_more from "../../../public/icons/icon_more.png";

function SearchFilterBar({ filter_icon_bool }) {
  const barHeight = {
    height: "66px",
  };
  return (
    <Stack
      className="main-blue-bg px-3"
      direction="horizontal"
      style={barHeight}
    >
      <Container className="w-auto ms-0 ms-xl-5">
        <FilterIcon is_active={filter_icon_bool} />
      </Container>
      <Container className="mx-auto">
        <SearchBar />
      </Container>
      <Container className="w-auto me-0 me-xl-5">
        <Image src={icon_more} alt="More action" />
      </Container>
    </Stack>
  );
}

SearchFilterBar.PropTypes = {
  filter_icon_bool: PropTypes.bool.isRequired,
};

export default SearchFilterBar;
