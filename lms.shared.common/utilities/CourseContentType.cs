namespace lms.shared.common.utilities
{
    [Serializable]
    public enum CourseContentType
    {
        // Image formats
        ImageJpeg = 1,
        ImagePng,
        ImageGif,
        ImageBmp,
        ImageTiff,
        ImageWebp,

        // Video formats
        VideoMp4,
        VideoMpeg,
        VideoQuicktime,
        VideoAvi,
        VideoMov,
        VideoWmv,
        VideoFlv,
        VideoWebm,

        // Document formats
        ApplicationPdf,
        ApplicationZip,
        TextPlain,
        ApplicationMsword,
        ApplicationVndOpenxmlformatsOfficedocumentWordprocessingmlDocument,
        ApplicationVndMsExcel,
        ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
        ApplicationVndMsPowerpoint,
        ApplicationVndOpenxmlformatsOfficedocumentPresentationmlPresentation
    }
}