import PropTypes from "prop-types";
import { Stack, Container, Button } from "react-bootstrap";
import Image from "next/image";
import SearchBar from "../smaller_components/search_bar";
import FilterIcon from "../switches/filter_switch";
import icon_more from "../../../public/icons/icon_more.png";

/**
 * Create Bar that holds filter button, search bar and more action icon.
 * @component
 * @param {object} props
 * @param {boolean} props.filter_icon_bool True if any filter or sort is active
 * @param {Function} props.moreButtonAction Action that will activate after clicking more action button
 * @param {Function} props.filterAction Action that will activate after clicking filter button
 * @return {JSX.Element} Stack element
 */
function SearchFilterBar({ filter_icon_bool, moreButtonAction, filterAction }) {
  // Styles
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
