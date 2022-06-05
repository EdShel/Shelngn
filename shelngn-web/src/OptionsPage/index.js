import clsx from "clsx";
import React, { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  deleteMember,
  deleteProject,
  deleteScreenshot,
  deleteUnpublishProject,
  getProjectInfo,
  postAddMember,
  postPublishProject,
} from "../api";
import LineLoader from "../components/LineLoader";
import ScreenLayout, { contentClassName } from "../components/ScreenLayout";
import useAuth from "../hooks/useAuth";
import UrlTo from "../UrlTo";
import useWorkspaceId from "../WorkspacePage/hooks/useWorkspaceId";
import ConfirmModal from "./ConfirmModal";
import DeleteButton from "./DeleteButton";
import plusIcon from "../assets/plus.svg";
import { useShowAlertNotification } from "../InfoAlert";
import AddMemberModal from "./AddMemberModal";
import MemberCard from "./MemberCard";
import styles from "./styles.module.css";
import PublishButton from "./PublishButton";
import ScreenshotsList from "./ScreenshotsList";
import Scrollable from "../components/Scrollable";
import { useTranslation } from "react-i18next";

const OptionsPage = () => {
  const workspaceId = useWorkspaceId();
  const [project, setProject] = useState(null);
  const [isLoading, setLoading] = useState(true);
  const [showDeleteProject, setShowDeleteProject] = useState(false);
  const [memberIdToDelete, setMemberIdToDelete] = useState(null);
  const [screenshotIdToDelete, setScreenshotIdToDelete] = useState(null);
  const [showAddMember, setShowAddMember] = useState(false);
  const { isLoading: isUserLoading, user } = useAuth();
  const [isPublishLoading, setPublishLoading] = useState(false);
  const navigate = useNavigate();
  const { showError, showInfo } = useShowAlertNotification();
  const {
    t,
    i18n: { language },
  } = useTranslation();

  const fetchProjectInfo = useCallback(async () => {
    try {
      const projectResponse = await getProjectInfo(workspaceId);
      setProject(projectResponse);
    } catch (e) {
      showError("Error while loading the project information.");
    }
  }, [workspaceId, showError]);

  const handleTogglePublish = useCallback(async () => {
    setPublishLoading(true);
    const isPublished = project.publication.isPublished;
    try {
      if (!isPublished) {
        await postPublishProject(workspaceId);
        setProject((oldState) => ({
          ...oldState,
          publication: {
            isPublished: true,
            date: new Date().toISOString(),
          },
        }));
      } else {
        await deleteUnpublishProject(workspaceId);
        setProject((oldState) => ({
          ...oldState,
          publication: {
            isPublished: false,
            date: null,
          },
        }));
      }
    } catch (e) {
      const responseError = e.response?.data.error;
      showError(responseError || "Cannot change state, please try again later");
    }
    setPublishLoading(false);
  }, [project?.publication, showError, workspaceId]);

  useEffect(() => {
    const fetchProject = async () => {
      fetchProjectInfo();
      setLoading(false);
    };
    fetchProject();
  }, [fetchProjectInfo]);

  return (
    <ScreenLayout>
      <LineLoader isLoading={isLoading || isUserLoading} />
      <Scrollable className={clsx(contentClassName, styles.container)}>
        {project && (
          <>
            <div className={styles.header}>
              <h1 className={styles["header-text"]}>{project.projectName}</h1>
              <DeleteButton onPress={() => setShowDeleteProject(true)} />
              <div className={styles["publish-button-container"]}>
                {project.publication.isPublished && (
                  <p>
                    {t("options.currentlyPublishedVersion")}{" "}
                    <b>{new Date(project.publication.date).toLocaleString(language)}</b>
                  </p>
                )}
                <PublishButton
                  isPublished={project.publication.isPublished}
                  onTogglePublish={handleTogglePublish}
                  isLoading={isPublishLoading}
                />
              </div>
            </div>

            <section>
              <h2>
                {t("options.members", { count: project.members.length })} ({project.members.length})
              </h2>
              <div className={styles.members}>
                {project.members.map((member) => (
                  <MemberCard
                    key={member.id}
                    userName={member.userName}
                    email={member.email}
                    role={member.memberRole}
                    isMe={member.id === user.id}
                    canBeDeleted={member.canBeDeleted}
                    onRemove={() => setMemberIdToDelete(member.id)}
                  />
                ))}
                <button className={styles["create-user-card"]} onClick={() => setShowAddMember(true)}>
                  <img src={plusIcon} alt="Add" />
                  {t("options.addNewMember")}
                </button>
              </div>
            </section>
            <ScreenshotsList
              onDeleteScreenshot={(id) => setScreenshotIdToDelete(id)}
              screenshots={project.screenshots}
            />
          </>
        )}
      </Scrollable>
      {showDeleteProject && (
        <ConfirmModal
          title={t("options.deleteProject")}
          text={t("options.confirmDeleteProject")}
          onCancel={() => setShowDeleteProject(false)}
          onOk={async () => {
            try {
              await deleteProject(workspaceId);
              navigate(UrlTo.home());
            } catch (ex) {
              showError(t("options.cannotDelete"));
            }
          }}
        />
      )}
      {memberIdToDelete && (
        <ConfirmModal
          title={t("options.removeMember")}
          text={t("options.confirmRemoveMember")}
          onOk={async () => {
            try {
              await deleteMember(workspaceId, memberIdToDelete);
              showInfo(t("options.removedMember"));
              setMemberIdToDelete(null);
              await fetchProjectInfo();
            } catch (ex) {
              showError(t("options.cannotRemoveMember"));
            }
          }}
          onCancel={() => setMemberIdToDelete(null)}
        />
      )}
      {screenshotIdToDelete && (
        <ConfirmModal
          title={t("options.deleteScreenshot")}
          text={t("options.confirmDeleteScreenshot")}
          onOk={async () => {
            try {
              await deleteScreenshot(workspaceId, screenshotIdToDelete);
              showInfo(t("options.removedScreenshot"));
              setScreenshotIdToDelete(null);
              setProject((oldProj) => ({
                ...oldProj,
                screenshots: oldProj.screenshots.filter((s) => s.id !== screenshotIdToDelete),
              }));
            } catch (ex) {
              showError(t("options.cannotDeleteScreenshot"));
            }
          }}
          onCancel={() => setScreenshotIdToDelete(null)}
        />
      )}
      {showAddMember && (
        <AddMemberModal
          onAddMember={async ({ emailOrUserName }) => {
            try {
              await postAddMember(workspaceId, emailOrUserName);
              showInfo(t("options.addedProjectMember"));
              setShowAddMember(false);
              await fetchProjectInfo();
            } catch (e) {
              const errorText = e.response?.data.Message;
              showError(errorText || t("options.cannotAddProjectMember"));
            }
          }}
          onCancel={() => setShowAddMember(false)}
        />
      )}
    </ScreenLayout>
  );
};

export default OptionsPage;
