import PropTypes from "prop-types";
import { Stack, Container, Button } from "react-bootstrap";
import Image from "next/image";
import SearchBar from "../smaller_components/search_bar";
import FilterIcon from "../switches/filter_switch";
import icon_more from "../../../public/icons/icon_more.png";

function SearchFilterBar({ filter_icon_bool, moreButtonAction, filterAction }) {
  const barHeight = {
    height: "66px",
  };
  return (
    <Stack
      className="main-blue-bg px-3 border-bottom-grey minScalableWidth"
      direction="horizontal"
      style={barHeight}
    >
      <Container className="w-auto ms-0 ms-xl-3">
        <FilterIcon is_active={filter_icon_bool} filterAction={filterAction} />
      </Container>
      <Container className="mx-auto">
        <SearchBar />
      </Container>
      <Container className="w-auto me-0 me-xl-3">
        <Button variant="as-link" className="p-0" onClick={moreButtonAction}>
          <Image src={icon_more} alt="More action" />
        </Button>
      </Container>
    </Stack>
  );
}

SearchFilterBar.propTypes = {
  filter_icon_bool: PropTypes.bool.isRequired,
  moreButtonAction: PropTypes.func.isRequired,
  filterAction: PropTypes.func.isRequired,
};

export default SearchFilterBar;
