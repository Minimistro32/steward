<script>
    import Card from "../ui/Card.svelte";
</script>

<Card title="Weekly Timeline">
    <div class="timeline">
        <div class="days">
            <span>Mon</span>
            <span>Tue</span>
            <span>Wed</span>
            <span>Thu</span>
            <span>Fri</span>
            <span>Sat</span>
            <span>Sun</span>
        </div>

        <div class="event" style="left:4%; width:22%;">Policy Review</div>

        <div class="event" style="left:18%; width:15%;">Maintenance</div>

        <div class="event" style="left:32%; width:38%;">Quarterly Audit</div>

        <div class="event" style="left:55%; width:12%;">Freeze</div>
    </div>
</Card>

<style>

    .timeline {
        position: relative;
        height: 180px;
    }

    .days {
        display: grid;
        grid-template-columns: repeat(7, 1fr);
        margin-bottom: var(--space-4);
        color: var(--color-text-muted);
        font-size: 0.8rem;
        text-align: center;
    }

    .days span {
        padding-bottom: var(--space-2);
        border-bottom: 1px solid var(--color-border);
    }

    .event {
        position: absolute;

        height: 34px;
        line-height: 34px;

        padding: 0 var(--space-3);

        background: rgba(134, 188, 73, 0.12);
        border: 1px solid rgba(134, 188, 73, 0.25);

        color: var(--color-brand-light);

        border-radius: var(--radius-sm);

        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .event:nth-child(2) {
        top: 48px;
    }
    .event:nth-child(3) {
        top: 92px;
    }
    .event:nth-child(4) {
        top: 136px;
    }
    .event:nth-child(5) {
        top: 70px;
    }
</style>
