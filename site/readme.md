---
title: Home
layout: simple
og_type: website
---

<section class="text-center py-5 text-white hero-text">
  <div class="container">
    <h1 class="fw-bold display-6">
      <span class="c64-text">XenoAtom.Logging</span>
    </h1>
    <p class="lead mt-3 mb-4">
      High-performance structured logging for .NET.<br>
      Zero-allocation hot path, predictable async behavior, and rich terminal/UI output.
    </p>
    <div class="d-flex justify-content-center gap-3 mt-4 flex-wrap">
      <a href="{{site.basepath}}/docs/getting-started/" class="btn btn-primary btn-lg"><i class="bi bi-rocket-takeoff"></i> Get started</a>
      <a href="{{site.basepath}}/docs/" class="btn btn-outline-light btn-lg"><i class="bi bi-book"></i> Browse docs</a>
      <a href="https://github.com/XenoAtom/XenoAtom.Logging/" class="btn btn-info btn-lg"><i class="bi bi-github"></i> GitHub</a>
    </div>
    <div class="row row-cols-1 row-cols-lg-2 gx-3 gy-3 mt-4 text-start mx-auto" style="max-width: 66rem;">
      <div class="col">
        <div class="card h-100">
          <div class="card-header h5"><i class="bi bi-box-seam lunet-feature-icon lunet-icon--input"></i>Core package</div>
          <div class="card-body">
            <p class="mb-2">Install <code>XenoAtom.Logging</code> for core runtime APIs, async processing, formatters, and file/JSON sinks.</p>
            <pre class="language-shell-session mb-0"><code>dotnet add package XenoAtom.Logging</code></pre>
          </div>
        </div>
      </div>
      <div class="col">
        <div class="card h-100">
          <div class="card-header h5"><i class="bi bi-display lunet-feature-icon lunet-icon--controls"></i>Terminal package (optional)</div>
          <div class="card-body">
            <p class="mb-2">Add <code>XenoAtom.Logging.Terminal</code> for markup logs, rich segment styling, and <code>LogControl</code> integration powered by <a href="https://xenoatom.github.io/terminal">XenoAtom.Terminal.UI</a>.</p>
            <pre class="language-shell-session mb-0"><code>dotnet add package XenoAtom.Logging.Terminal</code></pre>
          </div>
        </div>
      </div>
    </div>
    <div class="mt-4">
      <p class="mb-2 text-white-50"><code>LogControl</code> demo (markup + structured logs + background producer):</p>
      <video class="terminal img-fluid" autoplay muted loop playsinline preload="metadata" aria-label="XenoAtom.Logging LogControl demonstration">
        <source src="{{site.basepath}}/img/xenoatom-logcontrol.webm" type="video/webm">
        <img class="terminal img-fluid" src="{{site.basepath}}/img/screenshot.png" alt="XenoAtom.Logging terminal screenshot">
      </video>
    </div>
  </div>
</section>

<section class="container my-5">
  <h2 class="display-6 mb-4"><i class="bi bi-stars lunet-feature-icon lunet-icon--themes"></i>Features</h2>
  <div class="row row-cols-1 row-cols-lg-2 gx-4 gy-4">
    <div class="col">
      <div class="card h-100">
        <div class="card-header h4"><i class="bi bi-lightning-charge lunet-feature-icon lunet-icon--actions"></i> Zero-allocation hot path</div>
        <div class="card-body">
          <p class="card-text mb-2">Interpolation handlers and generated logging methods minimize allocations on enabled logging paths.</p>
          <p class="mb-0"><a href="{{site.basepath}}/docs/benchmarks/">Performance benchmarks</a></p>
        </div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header h4"><i class="bi bi-cpu lunet-feature-icon lunet-icon--layout"></i> Sync-first, async when needed</div>
        <div class="card-body">
          <p class="card-text mb-2">Use sync processing by default, or switch to async with explicit queue capacity and overflow policies.</p>
          <p class="mb-0"><a href="{{site.basepath}}/docs/getting-started/">Getting started guide</a></p>
        </div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header h4"><i class="bi bi-braces lunet-feature-icon lunet-icon--binding"></i> Structured + generated APIs</div>
        <div class="card-body">
          <p class="card-text mb-2">Use properties/scopes with `[LogMethod]` and `[LogFormatter]` generation for strongly typed APIs.</p>
          <p class="mb-0"><a href="{{site.basepath}}/docs/source-generator/">Source generator docs</a> · <a href="{{site.basepath}}/docs/log-formatter/">Formatter docs</a></p>
        </div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header h4"><i class="bi bi-file-earmark-text lunet-feature-icon lunet-icon--debug"></i> Production-ready sinks</div>
        <div class="card-body">
          <p class="card-text mb-2">File rolling, retention, failure handling, JSONL output, and runtime diagnostics.</p>
          <p class="mb-0"><a href="{{site.basepath}}/docs/file-writer/">File and JSON writers</a></p>
        </div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header h4"><i class="bi bi-terminal lunet-feature-icon lunet-icon--controls"></i> Terminal + LogControl rendering</div>
        <div class="card-body">
          <p class="card-text mb-2">Render rich logs, markup, and visuals in terminal apps and fullscreen UIs with <code>XenoAtom.Terminal.UI</code>.</p>
          <p class="mb-0"><a href="{{site.basepath}}/docs/terminal/">Terminal integration</a> · <a href="{{site.basepath}}/docs/terminal-visuals/">Visual examples</a></p>
        </div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header h4"><i class="bi bi-arrow-left-right lunet-feature-icon lunet-icon--themes"></i> Migration guidance</div>
        <div class="card-body">
          <p class="card-text mb-2">If you use <code>Microsoft.Extensions.Logging</code>, follow a practical migration path to equivalent patterns.</p>
          <p class="mb-0"><a href="{{site.basepath}}/docs/microsoft-extensions-logging/">Migration from MEL</a></p>
        </div>
      </div>
    </div>
  </div>
</section>
