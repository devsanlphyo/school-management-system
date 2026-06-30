# HelloTwo — Design System

> **Reference document** for all UI patterns, tokens, components, and conventions used across the HelloTwo Schools Management System. Keep this file updated whenever new components or patterns are introduced.

---

## 1. Brand & Identity

| Attribute | Value |
|---|---|
| **Brand Name** | `HelloTwo` |
| **Portal Titles** | `HelloTwo Admin`, `HelloTwo Headmaster` |
| **Page Title Format** | `{Page Name} — HelloTwo` |

---

## 2. Design Tokens (CSS Custom Properties)

All tokens are defined in `wwwroot/app.css` under `:root`.

### 2.1 Color Palette

#### Backgrounds
| Token | Value | Usage |
|---|---|---|
| `--bg-base` | `#f4f6fb` | Page/app background |
| `--bg-surface` | `#ffffff` | Cards, panels, header, sidebar |
| `--bg-panel` | `#ffffff` | Alias for surface |

#### Borders
| Token | Value |
|---|---|
| `--border-color` | `#e2e5f0` |
| `--border-2` | `#d0d4e2` (stronger, used on inputs) |

#### Text
| Token | Value | Usage |
|---|---|---|
| `--text-primary` | `#1a1d2e` | Headings, primary content |
| `--text-secondary` | `#3b3f54` | Supporting text, table body |
| `--text-muted` | `#7c8298` | Labels, captions, placeholder-like text |

#### Accent / Brand Colors
| Token | Value | Color |
|---|---|---|
| `--primary` | `#6366f1` | Indigo — main action color |
| `--primary-hover` | `#4f46e5` | Darker indigo on hover |
| `--primary-glow` | `rgba(99, 102, 241, 0.08)` | Subtle fill (active nav, focus rings) |
| `--accent-2` | `#8b5cf6` | Purple — gradient pair with `--primary` |

#### Semantic Colors
| Token | Value | Color |
|---|---|---|
| `--success` | `#10b981` | Emerald green |
| `--success-bg` | `rgba(16, 185, 129, 0.1)` | — |
| `--success-border` | `rgba(16, 185, 129, 0.2)` | — |
| `--warning` | `#f59e0b` | Amber |
| `--warning-bg` | `rgba(245, 158, 11, 0.1)` | — |
| `--warning-border` | `rgba(245, 158, 11, 0.2)` | — |
| `--danger` | `#f43f5e` | Rose red |
| `--danger-bg` | `rgba(244, 63, 94, 0.1)` | — |
| `--danger-border` | `rgba(244, 63, 94, 0.2)` | — |
| `--info` | `#06b6d4` | Cyan |
| `--info-bg` | `rgba(6, 182, 212, 0.1)` | — |
| `--info-border` | `rgba(6, 182, 212, 0.2)` | — |

### 2.2 Layout Sizes
| Token | Value |
|---|---|
| `--sidebar-width` | `220px` |
| `--header-height` | `70px` |

### 2.3 Border Radius
| Token | Value | Usage |
|---|---|---|
| `--radius-lg` | `14px` | Cards, panels, modals |
| `--radius-md` | `10px` | Tab items |
| `--radius-sm` | `8px` | Buttons, inputs |

### 2.4 Shadows
| Token | Value | Usage |
|---|---|---|
| `--shadow` | `0 1px 3px rgba(0,0,0,0.06), 0 4px 16px rgba(0,0,0,0.04)` | Default card shadow |
| `--shadow-lg` | `0 4px 24px rgba(0,0,0,0.08)` | Hover, modals, login card |

### 2.5 Transition
| Token | Value |
|---|---|
| `--transition-smooth` | `all 0.15s ease` |

---

## 3. Typography

### 3.1 Font Families

| Family | Weights | Usage |
|---|---|---|
| **Outfit** | 300–900 | All headings (h1–h6), brand title, stat values |
| **Plus Jakarta Sans** | 300–700 | Body text, buttons, inputs, labels |

Loaded via Google Fonts in `app.css`.

### 3.2 Type Scale

| Element | Font | Size | Weight | Notes |
|---|---|---|---|---|
| Page title (h2) | Outfit | `1.85rem` | 900 | `color: var(--text-primary)` |
| Card section title (h3) | Outfit | `1.1–1.15rem` | 800 | Often has bottom border divider |
| Page subtitle / description | Plus Jakarta Sans | `0.92rem` | 400 | `color: var(--text-muted)` |
| Stat value | Outfit | `1.8rem` | 800 | `letter-spacing: -1px` |
| Stat label | Plus Jakarta Sans | `0.75rem` | 500 | `color: var(--text-muted)` |
| Brand title in sidebar | Outfit | `1.15rem` | 900 | Gradient text (primary → accent-2) |
| Table content | Plus Jakarta Sans | `0.82rem` | 400/600 | — |
| Form label | Plus Jakarta Sans | `0.7rem` | 700 | Uppercase, `letter-spacing: 0.04em` |
| Form input | Plus Jakarta Sans | `0.835rem` | — | — |
| Badge | Plus Jakarta Sans | `0.68rem` | 700 | — |
| Profile name in header | Plus Jakarta Sans | `0.82rem` (mobile) / `0.9rem` (desktop) | 500 | `color: var(--text-secondary)` |

---

## 4. Layout System

### 4.1 App Shell

Both Admin and Headmaster portals share the same shell:

```
+------------------+---------------------------+
| Sidebar (220px)  |  Main Content             |
| .admin-sidebar   |  .admin-main              |
| - sidebar-header |  - admin-header (sticky)  |
| - sidebar-nav    |  - admin-content          |
| - sidebar-footer |                           |
+------------------+---------------------------+
```

- **Mobile** (< 992px): Sidebar hidden off-screen, slides in as a drawer on hamburger tap.
- **Desktop** (>= 992px): Sidebar always visible; main gets `margin-left: 220px`.

### 4.2 CSS Shell Classes

| Class | Description |
|---|---|
| `.admin-layout` | Root flex container |
| `.admin-sidebar` | Fixed sidebar |
| `.admin-sidebar.open` | Slides sidebar into view on mobile |
| `.sidebar-overlay` | Blur backdrop (mobile only) |
| `.sidebar-overlay.show` | Visible state of backdrop |
| `.sidebar-header` | Brand title area |
| `.sidebar-nav` | Scrollable nav links |
| `.sidebar-footer` | Logout button |
| `.admin-main` | Main content column |
| `.admin-header` | Sticky top bar (70px) |
| `.admin-content` | Inner content padding area |
| `.mobile-nav-toggle` | Hamburger button (hidden on desktop) |

### 4.3 Header Anatomy

**Admin Portal** (`AdminLayout.razor`):
- Left: hamburger toggle (mobile only)
- Right: `.profile-name` — logged-in user's `FullName` (**plain text, not a link**)

**Headmaster Portal** (`HeadmasterLayout.razor`):
- Left: hamburger + `.header-school` (school icon + school name)
- Right: `.profile-name` — headmaster's `FullName`

### 4.4 Content Padding

| Breakpoint | Padding |
|---|---|
| Mobile | `1rem` |
| Desktop (>= 992px) | `2rem` |

---

## 5. Components

### 5.1 Glass Panel (Surface Card)

Standard surface container for all content sections.

```html
<div class="glass-panel">
  <!-- content -->
</div>
```

| Property | Value |
|---|---|
| Background | `var(--bg-surface)` |
| Border | `1px solid var(--border-color)` |
| Border radius | `var(--radius-lg)` = 14px |
| Default padding | `1.5rem` |
| Shadow | `var(--shadow)` |

> Use `style="padding: 0; overflow: hidden;"` when wrapping a table to clip the table corners cleanly.

---

### 5.2 Premium Table

```html
<div class="glass-panel" style="padding: 0; overflow: hidden;">
  <div class="table-container">
    <table class="premium-table">
      <thead>
        <tr>
          <th>Column</th>
          <th style="text-align: right;">Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>Data</td>
          <td style="text-align: right;">
            <!-- icon buttons -->
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
```

**Empty State:**
```html
<tr>
  <td colspan="N" style="text-align: center; padding: 3rem; color: var(--text-muted);">
    No items found.
  </td>
</tr>
```

---

### 5.3 Stat Cards

```html
<div class="dashboard-grid">
  <div class="stat-card">
    <div style="color: var(--primary); margin-bottom: 0.75rem;">
      <!-- SVG icon 24x24 -->
    </div>
    <span class="stat-value">42</span>
    <span class="stat-label" style="margin-top: 0.25rem;">Total Teachers</span>
  </div>
</div>
```

`.dashboard-grid` uses `auto-fit`, min 200px per column, 1rem gap.  
Stat cards have a subtle radial glow in the top-right corner via `::after`.  
Hover: `translateY(-2px)` + `--shadow-lg`.

---

### 5.4 Badges

```html
<span class="badge badge-present">Present</span>
<span class="badge badge-late">Late</span>
<span class="badge badge-absent">Absent</span>
<span class="badge badge-pending">Pending</span>
<span class="badge badge-approved">Approved</span>
<span class="badge badge-rejected">Rejected</span>
<span class="badge badge-indigo">Admin Role</span>
<span class="badge badge-cyan">Info</span>
<span class="badge badge-green">Success</span>
<span class="badge badge-amber">Warning</span>
<span class="badge badge-rose">Danger</span>
```

| Class | Color Pair |
|---|---|
| `.badge-present` / `.badge-green` / `.badge-approved` | `--success` on `--success-bg` |
| `.badge-late` / `.badge-amber` / `.badge-pending` | `--warning` on `--warning-bg` |
| `.badge-absent` / `.badge-rose` / `.badge-rejected` | `--danger` on `--danger-bg` |
| `.badge-indigo` | `--primary` on `--primary-glow` |
| `.badge-cyan` | `--info` on `--info-bg` |

**Alert / Feedback Banner** (stretched badge):
```html
<div class="badge badge-green"
     style="padding: 0.75rem 1rem; border-radius: var(--radius-sm); font-size: 0.85rem;
            display: flex; align-items: center; gap: 0.5rem; width: 100%;">
  <!-- checkmark SVG 16x16 -->
  Saved successfully!
</div>
```

---

### 5.5 Buttons

**Primary (gradient):**
```html
<button class="btn-premium">Save Changes</button>
```

**Secondary / ghost:**
```html
<button class="btn-premium btn-secondary-custom">Cancel</button>
```

**Icon-only action (in tables):**
```html
<button class="btn-premium btn-secondary-custom" style="padding: 0.4rem;" title="Edit">
  <!-- SVG 14x14 -->
</button>
```

**Danger icon button:**
```html
<button class="btn-premium btn-secondary-custom"
        style="padding: 0.4rem; color: var(--danger); border-color: rgba(244,63,94,0.15);">
  <!-- trash SVG 14x14 -->
</button>
```

---

### 5.6 Form Controls

```html
<div class="form-group">
  <label class="form-label">Field Name</label>
  <InputText class="form-control-custom" @bind-Value="model.Field" placeholder="..." />
  <ValidationMessage For="@(() => model.Field)" class="text-danger"
                     style="font-size: 0.8rem; margin-top: 0.25rem; display: block;" />
</div>
```

Focus ring: `border-color: var(--primary)` + `box-shadow: 0 0 0 3px rgba(99,102,241,0.12)`.

**Toggle Switch (Maintenance Mode):**
```html
<label class="switch" style="position: relative; display: inline-block; width: 44px; height: 24px; flex-shrink: 0;">
  <input type="checkbox" @bind="isEnabled" style="opacity: 0; width: 0; height: 0;" />
  <span class="slider"
        style="position: absolute; cursor: pointer; top: 0; left: 0; right: 0; bottom: 0;
               background-color: var(--border-2); border-radius: 34px; transition: .3s;
               @(isEnabled ? "background-color: var(--danger);" : "")">
    <span style="position: absolute; height: 16px; width: 16px; left: 4px; bottom: 4px;
                 background-color: white; border-radius: 50%; transition: .3s;
                 @(isEnabled ? "transform: translateX(20px);" : "")"></span>
  </span>
</label>
```

---

### 5.7 Modal

```html
@if (showModal)
{
    <div class="modal-backdrop-custom">
        <div class="modal-content-custom">
            <div class="modal-header-custom">
                <h3 style="font-size: 1.1rem; font-weight: 800;">Title</h3>
                <button @onclick="CloseModal" style="background: none; border: none; cursor: pointer; color: var(--text-muted);">
                    <!-- X SVG 18x18 -->
                </button>
            </div>
            <div class="modal-body-custom">
                <!-- form content -->
            </div>
            <div class="modal-footer-custom">
                <button class="btn-premium btn-secondary-custom" @onclick="CloseModal">Cancel</button>
                <button class="btn-premium" @onclick="ConfirmAction">Confirm</button>
            </div>
        </div>
    </div>
}
```

| Class | Description |
|---|---|
| `.modal-backdrop-custom` | Fixed full-screen overlay, `backdrop-filter: blur(4px)` |
| `.modal-content-custom` | Card: 90% wide, max 500px, `--radius-lg`, `--shadow-lg` |
| `.modal-header-custom` | Title + close button, bottom border |
| `.modal-body-custom` | Scrollable area, max-height `70vh` |
| `.modal-footer-custom` | Button row, top border, right-aligned |

---

### 5.8 Tab Navigation

```html
<div class="tab-navigation">
  <button class="tab-link @(activeTab == "a" ? "active" : "")" @onclick='() => activeTab = "a"'>Tab A</button>
  <button class="tab-link @(activeTab == "b" ? "active" : "")" @onclick='() => activeTab = "b"'>Tab B</button>
</div>
```

`.tab-navigation` — pill container with `--bg-base` fill, border, rounded.  
`.tab-link.active` — white bg, `--shadow`, `--text-primary`.

---

### 5.9 Search Bar

```html
<div class="glass-panel" style="padding: 1rem; display: flex; align-items: center; gap: 1rem;">
  <div style="flex: 1; position: relative;">
    <input type="text" class="form-control-custom" placeholder="Search..."
           @bind-value="searchQuery" @bind-value:event="oninput"
           style="padding-left: 2.5rem;" />
    <!-- Search SVG 16x16 — position: absolute; left: 1rem; top: 50%; transform: translateY(-50%) -->
  </div>
</div>
```

---

### 5.10 Login Card

```html
<div class="login-container">
  <div class="login-card">
    <!-- Brand logo SVG 48x48 -->
    <!-- h2 "HelloTwo" + subtitle "Sign in to your account" -->
    <!-- EditForm with email + password + remember-me checkbox -->
    <!-- .btn-premium full-width "Sign In" button -->
  </div>
</div>
```

> **Note:** There is no Forgot Password link. Do not add one.

---

## 6. Page Structure Pattern

```html
<div class="d-flex flex-column" style="gap: 2rem;">

  <!-- 1. Page Header -->
  <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 1rem;">
    <div>
      <h2 style="font-size: 1.85rem; font-weight: 900; color: var(--text-primary); margin-bottom: 0.4rem;">
        Page Title
      </h2>
      <p style="color: var(--text-muted); font-size: 0.92rem;">Short description of this page.</p>
    </div>
    <button class="btn-premium" @onclick="OpenAddModal">
      <!-- Plus SVG 18x18 --> Add Item
    </button>
  </div>

  <!-- 2. Filter / Search Bar (optional) -->
  <div class="glass-panel" style="padding: 1rem; ...">...</div>

  <!-- 3. Data Table or Content Grid -->
  <div class="glass-panel" style="padding: 0; overflow: hidden;">
    <div class="table-container">
      <table class="premium-table">...</table>
    </div>
  </div>

  <!-- 4. Modal(s) -->
  @if (showModal) { ... }

</div>
```

For narrow, form-centric pages (Settings, Profile): use `max-width: 600px` on the outer `div`.

---

## 7. Icon Convention

All icons are **inline SVGs** in the **Feather Icons** visual style:

- `viewBox="0 0 24 24"` | `fill="none"` | `stroke="currentColor"` | `stroke-width="2"` | `stroke-linecap="round"` | `stroke-linejoin="round"`

| Context | Size |
|---|---|
| Sidebar navigation | `18×18` |
| Table action buttons | `14×14` |
| Dashboard stat cards | `24×24` |
| Header / school label | `20–22×20–22` |
| Alert / badge inline | `16×16` |
| Page header actions | `18×18` |

Color is inherited via `currentColor`. Set `style="color: var(--primary);"` on the SVG or a parent element to tint.

---

## 8. Navigation & Routing

### 8.1 URL Structure

| Route | Role | Portal |
|---|---|---|
| `/` or `/Account/Login` | All | Login |
| `/home` | Auth redirect hub | — |
| `/admin/schools` | Admin | Schools list |
| `/admin/schools/{id}` | Admin | School details |
| `/admin/users` | Admin | User management |
| `/admin/settings` | Admin | System settings |
| `/admin/profile` | Admin | Admin profile |
| `/headmaster/dashboard` | Headmaster, Officer, Admin | Dashboard |
| `/headmaster/teachers` | Headmaster, Officer, Admin | Teacher roster |
| `/headmaster/attendance` | Headmaster, Officer, Admin | Attendance |
| `/headmaster/leave-requests` | Headmaster, Officer, Admin | Leave requests |
| `/headmaster/lesson-plans` | Headmaster, Officer, Admin | Lesson plans |
| `/headmaster/settings` | Headmaster, Officer, Admin | Headmaster settings |

> **Naming Rule:** Use `headmaster` (not `admin`) for all school-level routes and UI titles.

### 8.2 Layout & Authorization Declaration

```razor
@page "/admin/schools"
@layout AdminLayout
@attribute [Authorize(Roles = "Admin")]
```

---

## 9. Roles

| Role | Portal | Notes |
|---|---|---|
| `Admin` | `/admin/...` | Global system admin; manages all schools and users |
| `Headmaster` | `/headmaster/...` | School-level head; read-only staff roster |
| `Officer` | `/headmaster/...` | Same portal access as Headmaster |
| `Director` | TBD | — |
| `Teacher` | Teacher Dashboard | Submits lesson plans, views own attendance |
| `Assistant` | TBD | Assigned to a school only (no department assignment) |

---

## 10. Blazor / EF Core Patterns

### 10.1 DbContext — Always Use Factory

```csharp
@inject IDbContextFactory<ApplicationDbContext> DbFactory

using var db = await DbFactory.CreateDbContextAsync();
var schools = await db.Schools.ToListAsync();
```

Never inject the scoped `ApplicationDbContext` directly into Blazor components.

### 10.2 UserManager — Use IServiceProvider

```csharp
@inject IServiceProvider ServiceProvider

using var scope = ServiceProvider.CreateScope();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var user = await userManager.FindByIdAsync(userId);
```

### 10.3 Current User Identity

```csharp
@inject AuthenticationStateProvider AuthStateProvider
@using System.Security.Claims

var authState = await AuthStateProvider.GetAuthenticationStateAsync();
var user = authState.User;
var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
```

### 10.4 Display Name Fallback

```csharp
displayName = !string.IsNullOrEmpty(appUser.FullName)
    ? appUser.FullName
    : (appUser.Email ?? "User");
```

---

## 11. Feedback Patterns

### 11.1 Empty State
```html
<td colspan="N" style="text-align: center; padding: 3rem; color: var(--text-muted);">
  No records found.
</td>
```

### 11.2 Inline Success Banner
```csharp
showSuccessAlert = true;
StateHasChanged();
await Task.Delay(3000);
showSuccessAlert = false;
StateHasChanged();
```

### 11.3 Inline Error Banner
```html
<div class="badge" style="background-color: rgba(239,68,68,0.1); color: var(--danger);
     padding: 0.75rem 1rem; border-radius: var(--radius-sm); font-size: 0.85rem;
     display: flex; align-items: center; gap: 0.5rem; width: 100%;">
  @errorMessage
</div>
```

### 11.4 Delete Confirmation Flow
1. `OpenDeleteModal(item)` — stores target, sets `showDeleteModal = true`
2. Modal shows item name in the message body
3. Confirm button calls delete method, then closes modal

---

## 12. Key Files Reference

| File | Purpose |
|---|---|
| `wwwroot/app.css` | All design tokens and global component CSS |
| `Components/Layout/AdminLayout.razor` | Shell for all `/admin/...` pages |
| `Components/Layout/HeadmasterLayout.razor` | Shell for all `/headmaster/...` pages |
| `Components/Account/Pages/Login.razor` | Login page |
| `Components/Pages/Admin/Schools.razor` | Reference: page header + table + modal pattern |
| `Components/Pages/Admin/Users.razor` | Reference: filter bar + complex modals |
| `Components/Pages/Admin/SchoolDetails.razor` | Reference: multi-card grid layout |
| `Components/Pages/Admin/Settings.razor` | Reference: toggle switch + narrow form layout |
| `Components/Pages/Admin/Profile.razor` | Reference: narrow form page with two sections |
| `Components/Pages/Headmaster/Dashboard.razor` | Reference: stat cards + action center cards |
| `Data/DbSeeder.cs` | Seeded default users and roles |

---

## 13. Feed Components

### 13.1 Feed Layout
The feed page layout utilizes `.feed-container` with a max-width of 680px for a focused timeline view.

### 13.2 Post Card
```html
<div class="glass-panel post-card">
  <div class="post-header">...</div>
  <div class="post-content">...</div>
  <div class="media-grid count-X">...</div>
  <div class="interaction-bar">...</div>
</div>
```
- Built on top of the `.glass-panel` component.
- Uses `--radius-lg` and `--shadow`.

### 13.3 Media Grid
Dynamic grid system `.media-grid` depending on the number of attachments:
- `.count-1`: 1 full-width item
- `.count-2`: 2 columns
- `.count-3`: 1 large top item + 2 smaller items below
- `.count-4`: 2x2 grid. If more than 4, the 4th item has a `.more-overlay`

### 13.4 Interaction & Comments
- **Reactions**: `.interact-btn` with `.loved` state for toggling the heart.
- **Comments**: `.comments-section` containing `.comment-item` and `.comment-bubble` for chat-like UI.
```
