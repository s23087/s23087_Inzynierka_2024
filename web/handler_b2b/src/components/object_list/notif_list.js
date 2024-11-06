"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import NotificationContainer from "../object_container/notification_container";
import switchNotifStatus from "@/utils/notifs/switch_notif_status";
import { useRouter } from "next/navigation";

/**
 * Return component that showcase notification objects, search bar, filter, more action element and selected element.
 * @component
 * @param {object} props Component props
 * @param {Array<{notificationId: Number, info: string, objectType: string, reference: string, isRead: boolean}>} props.notifs Array containing notification objects.
 * @param {Number} props.notifStart Starting index of notifications subarray.
 * @param {Number} props.notifEnd Ending index of notifications subarray.
 * @return {JSX.Element} Container element
 */
function NotificationList({ notifs, notifStart, notifEnd }) {
  const router = useRouter();
  return (
    <Container
      className="px-0 middleSectionPlacement-no-footer position-relative"
      fluid
    >
      {Object.keys(notifs ?? []).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">
            {notifs
              ? "Notifications not found :/"
              : "Could not connect to server."}
          </p>
        </Container>
      ) : (
        Object.values(notifs ?? [])
          .slice(notifStart, notifEnd)
          .map((value) => {
            return (
              <NotificationContainer
                key={value.notificationId}
                notification={value}
                switch_read_action={async () => {
                  await switchNotifStatus(value.notificationId, value.isRead);
                  router.refresh();
                }}
              />
            );
          })
      )}
    </Container>
  );
}

NotificationList.propTypes = {
  notifs: PropTypes.object.isRequired,
  notifStart: PropTypes.number.isRequired,
  notifEnd: PropTypes.number.isRequired,
};

export default NotificationList;
