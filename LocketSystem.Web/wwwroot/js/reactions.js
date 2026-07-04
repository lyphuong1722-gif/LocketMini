// ============================================================
// reactions.js — xử lý bảng chọn cảm xúc (like / love / haha / wow / sad / angry)
// trên mỗi bài viết ở trang Feed.
//
// LƯU Ý: Backend hiện tại chỉ hỗ trợ 1 loại "Like" (không phân biệt
// loại cảm xúc). Vì vậy: chọn cảm xúc nào cũng sẽ submit form Like
// có sẵn (nếu chưa like) để server ghi nhận lượt thích thật.
// Icon/emoji hiển thị được lưu trong localStorage theo từng postId
// để giữ lựa chọn khi tải lại trang trên CÙNG một trình duyệt.
// ============================================================

(function () {
    const REACTIONS = [
        { key: "like", emoji: "👍", label: "Thích" },
        { key: "love", emoji: "❤️", label: "Yêu thích" },
        { key: "haha", emoji: "😆", label: "Haha" },
        { key: "wow", emoji: "😮", label: "Wow" },
        { key: "sad", emoji: "😢", label: "Buồn" },
        { key: "angry", emoji: "😡", label: "Phẫn nộ" },
    ];

    function storageKey(postId) {
        return "locket-reaction-" + postId;
    }

    function applyReaction(trigger, reactionKey) {
        const reaction = REACTIONS.find(r => r.key === reactionKey) || REACTIONS[0];
        const iconEl = trigger.querySelector(".reaction-current-icon");
        const labelEl = trigger.querySelector(".reaction-current-label");
        if (iconEl) iconEl.textContent = reaction.emoji;
        if (labelEl) labelEl.textContent = reaction.label;
        trigger.classList.add("has-reacted");
    }

    function clearReaction(trigger) {
        const iconEl = trigger.querySelector(".reaction-current-icon");
        const labelEl = trigger.querySelector(".reaction-current-label");
        if (iconEl) iconEl.textContent = "🤍";
        if (labelEl) labelEl.textContent = "Thích";
        trigger.classList.remove("has-reacted");
    }

    function initReactionWrap(wrap) {
        const trigger = wrap.querySelector(".reaction-trigger");
        const bar = wrap.querySelector(".reaction-bar");
        if (!trigger || !bar) return;

        const postId = trigger.dataset.postId;
        const isLikedByServer = trigger.dataset.liked === "true";

        // Khôi phục lựa chọn cảm xúc đã lưu (chỉ khi server xác nhận đã like)
        const saved = localStorage.getItem(storageKey(postId));
        if (isLikedByServer) {
            applyReaction(trigger, saved || "like");
        } else {
            clearReaction(trigger);
            localStorage.removeItem(storageKey(postId));
        }

        let hideTimer = null;
        const showBar = () => {
            clearTimeout(hideTimer);
            bar.classList.add("show");
        };
        const scheduleHide = () => {
            hideTimer = setTimeout(() => bar.classList.remove("show"), 250);
        };

        trigger.addEventListener("mouseenter", showBar);
        trigger.addEventListener("mouseleave", scheduleHide);
        bar.addEventListener("mouseenter", showBar);
        bar.addEventListener("mouseleave", scheduleHide);

        // Hỗ trợ mobile: bấm giữ (long-press) để mở bảng cảm xúc
        let pressTimer = null;
        trigger.addEventListener("touchstart", () => {
            pressTimer = setTimeout(showBar, 350);
        });
        trigger.addEventListener("touchend", () => clearTimeout(pressTimer));

        // Chọn 1 emoji trong bảng
        bar.querySelectorAll(".reaction-emoji").forEach(btn => {
            btn.addEventListener("click", (e) => {
                e.preventDefault();
                const key = btn.dataset.reaction;

                localStorage.setItem(storageKey(postId), key);
                applyReaction(trigger, key);
                bar.classList.remove("show");

                // Nếu bài viết chưa được like ở server -> submit form Like thật
                if (!isLikedByServer) {
                    const form = trigger.closest("form");
                    if (form) form.requestSubmit();
                }
            });
        });

        // Bấm trực tiếp icon chính (không mở bảng) = toggle Like/Unlike như cũ
        trigger.addEventListener("click", (e) => {
            if (bar.classList.contains("show")) {
                // đang mở bảng thì click nền không làm gì (đợi chọn emoji)
                return;
            }
            // để form tự submit bình thường (Like/Unlike có sẵn)
        });
    }

    document.addEventListener("DOMContentLoaded", () => {
        document.querySelectorAll(".reaction-wrap").forEach(initReactionWrap);
    });
})();
