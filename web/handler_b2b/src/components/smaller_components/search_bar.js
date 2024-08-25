"use client";

import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { Container, Form, Button, Stack, InputGroup } from "react-bootstrap";
import Image from "next/image";
import search_icons from "../../../public/icons/icon_search.png";

export default function SearchBar() {
  const router = useRouter();
  const pathName = usePathname();
  const params = useSearchParams();
  return (
    <Container className="sec-blue-bg barStyle">
      <Form>
        <Stack direction="horizontal">
          <InputGroup>
            <Form.Control
              className="input-style-search"
              placeholder="Search"
              aria-label="Search"
              id="searchInput"
            />
          </InputGroup>
          <Button
            variant="as-link"
            className="ms-auto pe-0 py-0"
            onClick={(e) => {
              e.preventDefault();
              let variable = document.getElementById("searchInput");
              const newParams = new URLSearchParams(params);
              newParams.set("searchQuery", variable.value);
              newParams.set("page", 1);
              router.replace(`${pathName}?${newParams}`);
              variable.value = "";
            }}
          >
            <Image src={search_icons} alt="search icon" />
          </Button>
        </Stack>
      </Form>
    </Container>
  );
}
