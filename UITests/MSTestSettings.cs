// UI Automation tests drive real keyboard and mouse input against a window that has to hold
// focus, so they can only ever run one at a time. MSTest doesn't parallelize unless asked, but
// this makes it explicit for whatever gets added here next.
[assembly: DoNotParallelize]
