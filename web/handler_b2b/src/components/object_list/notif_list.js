"use client";

import PropTypes from "prop-types";
import { Container } from "react-bootstrap";
import NotificationContainer from "../object_container/notification_container";
import switchNotifStatus from "@/utils/notifs/switch_notif_status";
import { useRouter } from "next/navigation";

function NotificationList({ notifs, notifStart, notifEnd }) {
  const router = useRouter();
  return (
    <Container
      className="p-0 middleSectionPlacement-no-footer position-relative"
      fluid
    >
      {Object.keys(notifs).length === 0 ? (
        <Container className="text-center" fluid>
          <p className="mt-5 pt-5 blue-main-text h2">
            Notifications not found :/
          </p>
        </Container>
      ) : (
        Object.values(notifs)
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

NotificationList.PropTypes = {
  notifs: PropTypes.object.isRequired,
  notifStart: PropTypes.number.isRequired,
  notifEnd: PropTypes.number.isRequired,
};

export default NotificationList;
