/** @type {import('tailwindcss').Config} */
export const content = [
  "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
  "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
  "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
];
export const theme = {
  borderRadius: {
    none: "0",
    sm: "0.125rem",
    DEFAULT: "0.25rem",
    md: "0.35rem",
    lg: "0.4rem",
    full: "9999px",
    large: "12px",
  },
  colors: {
    main_blue: {
      primary: "#307AE9",
      secondary: "#24559E",
      primary_hover: "#0260EC",
      secondary_hover: "#0848A9",
    },
    bg_color: "#FFFFFF",
    gray: {
      primary: "#D9D9D9",
      secondary: "#989898",
      super_dark: "#707070",
    },
    black: "#2F3032",
    red: {
      primary: "#DD0000",
      secondary: "#9A0000",
    },
    green: "#22D21F",
    yellow: "#FAF473",
  },
  extend: {
    spacing: {
      minWidth: {
        128: "25",
      },
    },
  },
};
export const plugins = [];
