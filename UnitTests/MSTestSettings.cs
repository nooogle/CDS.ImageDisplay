// This is a UI suite: tests construct WinForms controls, run STA threads and share
// process-wide WinForms and GDI+ state, so they run one at a time.
[assembly: DoNotParallelize]
[assembly: DiscoverInternals]
